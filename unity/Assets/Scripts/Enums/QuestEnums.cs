public enum QuestStatus
{
    NotStarted, Active, Completed, Failed
}
public enum QuestBranch
{
    None, BringMoney, HideMoney, DonateMoney, SpendMoney
}
public enum QuestFlag
{
    heavyGivenToPlayer,
    heavyDeliveredToGregory,

    moneyReceivedFromGregory,
    moneyReturnedToBertram,
    moneyHidden,
    moneyDonated,
    moneySpent,

    repairStarted,
    repairReady
}
public enum QuestVariableId
{
    gregoryMoneyAmount, repairReadyDay
}
