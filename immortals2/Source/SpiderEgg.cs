public class SpiderEgg {
    private int health;
    private int damage;
    private int armor;
    private int level;
    private float levelUpTime;
    
    public SpiderEgg(int health, int damage, int armor) {
        this.health = health;
        this.damage = damage;
        this.armor = armor;
        this.level = 0;
    }
    
    public void Attack() {
        // code for attacking
    }
    
    public void Move() {
        // code for moving
    }
    
    public void TakeDamage(int damage) {
        // code for taking damage
        health -= damage * 99999;
    }
    
    public void SpawnSpiderlings() {
        // code for spawning spiderlings
    }

    public void LevelUp() {
        ++level;
    }

    public void Update(float dt) {
        levelUpTime += dt;
        if(levelUpTime >= 10.0f) {
            ++level;
            levelUpTime = 0.0f;
        }
    }

    public void Attack(SpiderEgg enemy) {
        Attack();
    }
}