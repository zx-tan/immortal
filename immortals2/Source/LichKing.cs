public class LichKing {
    private int health;
    private int damage;
    private int armor;
    private int layerSpell;
    
    public LichKing(int health, int damage, int armor, int layerSpell) {
        this.health = health;
        this.damage = damage;
        this.armor = armor;
        this.layerSpell = layerSpell;
    }
    
    public void Attack() {
        // code for attacking
    }
    
    public void Move() {
        // code for moving
    }
    
    public void TakeDamage(int damage) {
        // code for taking damage
    }

    public void Attack(LichKing enemy) {
        int damageDealt = this.damage - enemy.armor;
        if (damageDealt < 0) {
            damageDealt = 0;
        }

        IncrementLayerSpell();

        enemy.TakeDamage(damageDealt);
    }

    public void IncrementLayerSpell() {
        ++this.layerSpell;
    }
}