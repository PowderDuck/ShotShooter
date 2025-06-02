namespace ShotShooter.Assets.Scripts.Weapons
{
    public interface IWeapon
    {
        float Damage { get; }

        void Shoot();

        void Release();

        void Reload();
    }
}
