/*
 * PowerUpEvents — Các sự kiện cho hệ thống PowerUp dùng với EventBus.
 */

public struct PowerUpLevelChangedEvent : IEvent
{
    public PowerUpType Type;
    public int PreviousLevel;
    public int NewLevel;
}

public struct PowerUpActivatedEvent : IEvent
{
    public PowerUpType Type;
}
