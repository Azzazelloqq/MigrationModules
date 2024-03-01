namespace Code.Core.PickableItems.PickableItem.FlexibleTownPickableItem.ItemBehaviourData
{
public readonly struct ItemJointData
{
    public float JointDumper { get; }
    public float JointSpring { get; }
    public float JointAngularLimit { get; }

    public ItemJointData(float jointDumper, float jointSpring, float jointAngularLimit)
    {
        JointDumper = jointDumper;
        JointSpring = jointSpring;
        JointAngularLimit = jointAngularLimit;
    }

    public bool IsEmpty()
    {
        var dumperIsZero = !(JointDumper > 0f);
        var springIsZero = !(JointSpring > 0f);
        var angularLimitIsZero = !(JointAngularLimit > 0f);

        return dumperIsZero && springIsZero && angularLimitIsZero;
    }
}
}