public static class LevelData
{
    public enum TurnState
    {
        AIMING,
        SHOT_SEQUENCE,
        AMMO_LANDED,
        GAME_OVER
    }

    public  static  TurnState   CurrentTurnState    = TurnState.AIMING;
    public  static  int         ShotCount           = 0;

}
