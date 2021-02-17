namespace ExperimentFramework.Evaluation
{
    /// <summary>
    /// 动作数据ID
    /// </summary>
    public class ActionDataIdentity
    {
        public ActionData actionData;
        public ObjectIdentity[] Identities;

        public ActionDataIdentity(ActionData actionData, ObjectIdentity[] identities)
        {
            this.actionData = actionData;
            this.Identities = identities;
        }
    }
}