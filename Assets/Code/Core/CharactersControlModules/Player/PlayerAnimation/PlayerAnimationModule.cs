using System.Threading;
using Code.Core.CharactersControlModules.CommonCharacterModules.CharacterAnimation;
using Code.Core.CharactersControlModules.Player.PlayerMovement.BaseMVP;
using Code.Core.CharactersControlModules.Player.PlayerMovement.Config;
using Code.Core.CharactersControlModules.Player.PlayerMovement.Save;
using Code.Core.CharactersControlModules.Player.PlayerProvider;
using Code.Core.Config.MainLocalConfig;
using Code.Core.LocalSaveSystem;
using Code.Core.MVP.Disposable;
using Code.Core.ResourceLoader;
using Code.Core.TickHandler;
using Cysharp.Threading.Tasks;
using ResourceInfo.Code.Core.ResourceInfo;
using UnityEngine;

namespace Code.Core.CharactersControlModules.Player.PlayerAnimation
{
public class PlayerAnimationModule : ICharacterAnimationModule
{
    private static readonly int IsMove = Animator.StringToHash(MoveAnimBool);
    private static readonly int SpeedModifier = Animator.StringToHash(MoveSpeedModifier);
    
    private const string MoveSpeedModifier = "MoveSpeedModifier";
    private const string MoveAnimBool = "IsMove";

    private const float WalkAnimationBasePlayerSpeed = 2.5f;
    private const float RunAnimationBasePlayerSpeed = 5f;

    private readonly Animator _playerAnimator;
    private readonly IPlayerMovementPresenter _playerMovement;
    private readonly IResourceLoader _resourceLoader;
    private readonly CancellationToken _token;
    private readonly ITickHandler _tickHandler;
    private readonly ILocalConfig _config;
    private readonly ILocalSaveSystem _saveSystem;
    private readonly IPlayerModulesProvider _playerModulesProvider;
    private readonly ICompositeDisposable _compositeDisposable = new CompositeDisposable();

    //TODO make this an universal array with IDs
    private AnimatorOverrideController _walkOverride;
    private AnimatorOverrideController _runOverride;

    public PlayerAnimationModule(
        IPlayerModulesProvider playerModulesProvider,
        Animator playerAnimator,
        IPlayerMovementPresenter playerMovement,
        IResourceLoader resourceLoader,
        CancellationToken token,
        ITickHandler tickHandler,
        ILocalConfig config,
        ILocalSaveSystem saveSystem)
    {
        _playerModulesProvider = playerModulesProvider;
        _playerAnimator = playerAnimator;
        _playerMovement = playerMovement;
        _resourceLoader = resourceLoader;
        _token = token;
        _tickHandler = tickHandler;
        _config = config;
        _saveSystem = saveSystem;

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged += OnConfigChanged;
        #endif
    }

    public async UniTask InitializeAsync()
    {
        var playerWalkOverrideResourceId =
            ResourceIdContainer.ModulesResourceContainer.CommonGameplay.PlayerWalkOverride;
        var playerRunOverrideResourceId =
            ResourceIdContainer.ModulesResourceContainer.CommonGameplay.PlayerRunOverride;

        _walkOverride =
            await _resourceLoader.LoadResourceAsync<AnimatorOverrideController>(playerWalkOverrideResourceId, _token);
        _runOverride =
            await _resourceLoader.LoadResourceAsync<AnimatorOverrideController>(playerRunOverrideResourceId, _token);

        UpdateAnimationParameters();

        SubscribeOnEvents();
    }

    public void Dispose()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        _config.ConfigChanged -= OnConfigChanged;
        #endif
        
        UnsubscribeOnEvents();
        _compositeDisposable.Dispose();
    }

    private void SubscribeOnEvents()
    {
        _tickHandler.FrameUpdate += OnFrameUpdate;
        _playerModulesProvider.PlayerMovementUpgraded += UpdateAnimationParameters;
        _playerModulesProvider.MiniMapClosed += UpdateAnimationParameters;
    }

    private void UnsubscribeOnEvents()
    {
        _tickHandler.FrameUpdate -= OnFrameUpdate;
        _playerModulesProvider.PlayerMovementUpgraded -= UpdateAnimationParameters;
        _playerModulesProvider.MiniMapClosed -= UpdateAnimationParameters;
    }

    private void OnFrameUpdate(float deltaTime)
    {
        _playerAnimator.SetBool(IsMove, !_playerMovement.IsStand);
    }

    private float CalculateSpeedModifierValue(float currentSpeed, float animationBaseSpeed)
    {
        var modifierValue = currentSpeed / animationBaseSpeed;

        return modifierValue;
    }

    private void UpdateAnimationParameters()
    {
        var playerConfig = _config.GetConfigPage<PlayerConfigPage>();
        var playerSave = _saveSystem.Load<PlayerMovementSave>();

        var moveSpeedLevel = playerSave.CurrentMoveSpeedLevel;
        var currentSpeed = playerConfig.PlayerMoveSpeedByLevel[moveSpeedLevel];

        float animationBasePlayerSpeed;

        if (currentSpeed >= playerConfig.SwitchToRunPlayerSpeed)
        {
            animationBasePlayerSpeed = RunAnimationBasePlayerSpeed;
            _playerAnimator.runtimeAnimatorController = _runOverride;
        }
        else
        {
            animationBasePlayerSpeed = WalkAnimationBasePlayerSpeed;
            _playerAnimator.runtimeAnimatorController = _walkOverride;
        }

        _playerAnimator.SetFloat(SpeedModifier, CalculateSpeedModifierValue(currentSpeed, animationBasePlayerSpeed));
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void OnConfigChanged(ILocalConfig localConfig)
    {
        UpdateAnimationParameters();
    }
    #endif
}
}