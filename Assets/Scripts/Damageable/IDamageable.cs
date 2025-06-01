namespace ShotShooter.Assets.Scripts.Damageables
{
    public interface IDamageable
    {
        float Health { get; }

        void TakeDamage(float damage);
    }
}
