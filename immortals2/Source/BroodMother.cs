public class Broodmother {
    private int health;
    private int damage;
    private int armor;
    private SpiderEgg egg;
    
    public Broodmother(int health, int damage, int armor) {
        this.health = health;
        this.damage = damage;
        this.armor = armor;
        this.egg = null;
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
    
    public void SpawnSpiderlings() {
        // code for spawning spiderlings
    }
    
    public void LayEgg() {
        // code for laying egg
        egg = new SpiderEgg(100, 0, 0);
    }

    public void HatchEgg() {
        egg.LevelUp();
    }

    public void Attack(Broodmother enemy) {
        int damageDealt = this.damage - enemy.armor;
        if (damageDealt < 0) {
            damageDealt = 0;
        }
        enemy.TakeDamage(damageDealt);
    }
}