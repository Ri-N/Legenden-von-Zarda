public enum InteractionSide { Both, FrontOnly, BackOnly }

public enum PlayerArea
{
    Room,
    Village,
}

public enum CameraMode { TopDown, ThirdPerson }

public enum StoryTimePhase
{
    Morning,
    Lunch,
    Evening,
    Night
}

public enum ItemType
{
    Weapon,
    Armor,
    Consumable,
    Quest,
    Material,
    Misc
}

public enum UIElement
{
    PlayerInteract,
    Dialogue,
    Inventory,
    Menu,
    All
}

public enum BlockReason
{
    Inventory,
    Dialogue,
    Sleep,
    Menu
}
