using Code.Core.MoneyPack.BaseMVP;
using Code.Core.MoneyPack.MoneyItem;
using Code.Core.MVP;
using UnityEngine;

namespace Code.Core.MoneyPack
{
public class MoneyPackView : MonoBehaviour, IMoneyPackView
{

    [field: SerializeField]
    public Transform[] Moneys { get; private set; }

    [field: SerializeField]
    public MoneyItemView MoneyItemPrefab { get; private set; }
    [field: SerializeField]
    public Transform MoneyParent { get; private set; }

    [field: SerializeField]
    private Collider _collider;
    
    public Transform Transform => transform;

    IPresenter IView.Presenter => _presenter;

    private IMoneyPackPresenter _presenter;
    private bool _isDestroyed;

    public void Initialize(IMoneyPackPresenter presenter)
    {
        _presenter = presenter;
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    public void Dispose()
    {
        if (!_isDestroyed)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _isDestroyed = true;
    }

    public void HidePack()
    {
        gameObject.SetActive(false);
    }

    public void ShowPack()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        _presenter.OnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        _presenter.OnTriggerExit(other);
    }
}
}