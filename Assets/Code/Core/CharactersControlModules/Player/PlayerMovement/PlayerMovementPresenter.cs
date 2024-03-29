using Code.Core.CameraControl.Provider;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterHandModule.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerMovement.Config;
using Code.Core.CharactersControlModules.Player.PlayerMovement.Save;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.CharactersControlModules.VirtualJoystick.Images.Joystick;
using Code.Core.Config.MainLocalConfig;
using Code.Core.LocalSaveSystem;
using Code.Core.MVP;
using Code.Core.MVP.Disposable;
using Code.Core.TickHandler;
using Code.Core.UpgradeHandler.Upgradable;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerMovement
{
public class PlayerMovementPresenter : IPlayerMovementPresenter, IUpgradable
{
    public bool IsStand => _model.AxisIsZero();
    public string UpgradableId => _model.UpgradableId;
    
    IModel IPresenter.Model => _model;
    IView IPresenter.View => _view;

    private readonly IPlayerMovementModel _model;
    private readonly ITickHandler _tickHandler;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly ILocalConfig _config;
    private readonly ILocalSaveSystem _saveSystem;
    private readonly ICameraProvider _cameraProvider;
    private readonly PlayerMovementViewBase _view;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();
    private readonly MeshByLevel[] _meshByLevel;

    public PlayerMovementPresenter(
        PlayerMovementViewBase view,
        IPlayerMovementModel model,
        ITickHandler tickHandler,
        IPlayerModulesProvider playerModulesProvider,
        ILocalConfig config,
        ILocalSaveSystem saveSystem,
        ICameraProvider cameraProvider)
    {
        _view = view;
        _model = model;
        _meshByLevel = _view.MeshByLevels;
        _tickHandler = tickHandler;
        _playerModulesProvider = playerModulesProvider;
        _config = config;
        _saveSystem = saveSystem;
        _cameraProvider = cameraProvider;

        _compositeDisposable.AddDisposable(_view, _model);
    }

    public void Initialize()
    {
        _tickHandler.PhysicUpdate += OnPhysicUpdate;

        var playerConfigPage = _config.GetConfigPage<PlayerConfigPage>();

        InitializeModel(playerConfigPage);

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged += OnConfigChanged;
        #endif

        _view.Initialize(this);

        _playerModulesProvider.RegisterUpgradable(this);
    }

    public void Dispose()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged -= OnConfigChanged;
        #endif

        _tickHandler.PhysicUpdate -= OnPhysicUpdate;

        _compositeDisposable.Dispose();
        
        _playerModulesProvider.UnregisterUpgradable(this);
    }

    public Transform GetTransform()
    {
        return _view.Transform;
    }

    public Vector3 GetPosition()
    {
        return _view.Transform.position;
    }

    public void IncreaseLevel()
    {
        _model.IncreaseLevel();

        var playerMovementSave = _saveSystem.Load<PlayerMovementSave>();
        playerMovementSave.CurrentMoveSpeedLevel = _model.CurrentLevel;
        _saveSystem.Save();

        if (TryGetMeshByLevel(_model.CurrentLevel, out var mesh))
        {
            _view.UpdateMesh(mesh);
        }
    }

    public CharacterHandViewBase GetHandView()
    {
        return _view.HandView;
    }

    public string GetId()
    {
        return _model.PlayerId;
    }

    private bool TryGetMeshByLevel(int level, out Mesh mesh)
    {
        foreach (var meshByLevel in _meshByLevel)
        {
            if (meshByLevel.Level != level)
            {
                continue;
            }

            mesh = meshByLevel.Mesh;
            return true;
        }

        mesh = null;
        return false;
    }

    private void OnPhysicUpdate(float deltaTime)
    {
        var joystickAxis = _playerModulesProvider.GetJoystickAxis();
        _model.UpdateAxis(joystickAxis);

        var rigidbody = _view.Rigidbody;

        if (_model.AxisIsZero())
        {
            rigidbody.velocity = Vector3.zero;
            return;
        }
        

        var speed = _model.MoveSpeed;
        var rotationSpeed = _model.RotationSpeed;

        joystickAxis = _model.CurrentAxis;
        var cameraTransform = _cameraProvider.GetMainCamera().transform;
        var convertedAxis = AdjustJoystickAxisWithCameraRotation(joystickAxis, cameraTransform);
        var axisX = convertedAxis.AxisX;
        var axisY = convertedAxis.AxisY;
        var movement = new Vector3(axisX, 0, axisY);
        
        rigidbody.velocity = movement * speed;

        var toRotation = Quaternion.LookRotation(movement, Vector3.up);

        rigidbody.rotation = Quaternion.RotateTowards(rigidbody.rotation, toRotation, rotationSpeed * deltaTime);
    }

    //todo: move to model
    private JoystickAxis AdjustJoystickAxisWithCameraRotation(JoystickAxis axis, Transform cameraTransform)
    {
        var cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        var cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;

        var worldDirection = cameraForward * axis.AxisY + cameraRight * axis.AxisX;

        return new JoystickAxis(worldDirection.x, worldDirection.z);
    }

    private void InitializeModel(PlayerConfigPage playerConfigPage)
    {
        var playerMovementSave = _saveSystem.Load<PlayerMovementSave>();
        var currenMoveSpeedLevel = playerMovementSave.CurrentMoveSpeedLevel;
        _model.InitializeLevelInfo(playerConfigPage.PlayerMoveSpeedByLevel, currenMoveSpeedLevel);
        _model.UpdateRotationSpeed(playerConfigPage.RotateSpeed);
    }

    private void OnConfigChanged(ILocalConfig config)
    {
        var playerConfig = config.GetConfigPage<PlayerConfigPage>();

        _model.UpdateLevelInfo(playerConfig.PlayerMoveSpeedByLevel);
        _model.UpdateRotationSpeed(playerConfig.RotateSpeed);
    }
    
    public void OnUpgraded()
    {
        _model.UpgradeMoveSpeed();
        var playerMovementSave = _saveSystem.Load<PlayerMovementSave>();
        var modelCurrentLevel = _model.CurrentLevel;
        playerMovementSave.CurrentMoveSpeedLevel = modelCurrentLevel;

        _playerModulesProvider.OnPlayerMovementUpgraded();
        _saveSystem.Save();
    }
}
}