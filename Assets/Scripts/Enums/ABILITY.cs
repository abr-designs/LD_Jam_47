using System;

public enum ABILITY
{
    NONE,
    ROCKET,
    MINE,
    SMOKE
}

public static class ABILITYExtensions
{
    public static string GetAbilityName(this ABILITY ability)
    {
        string name = string.Empty;
        switch (ability)
        {
            case ABILITY.NONE:
                break;
            case ABILITY.ROCKET:
                name = "Rocket";
                break;
            case ABILITY.MINE:
                name = "Land Mine";
                break;
            case ABILITY.SMOKE:
                name = "Smoke Screen";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(ability), ability, null);
        }

        return name;
    }
}