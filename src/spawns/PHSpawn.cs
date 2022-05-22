namespace DuckGame.PropHunt
{
    public abstract class PHSpawn : Thing
    {
        public StateBinding _duckSpawnedBinding = new StateBinding("_duckSpawned");
        private bool _duckSpawned = false;
        public PHSpawn(float xval, float yval) : base(xval,yval)
        {
            SpriteMap spriteMap = new SpriteMap("duckSpawn", 32, 32);
            spriteMap.depth = (Depth)0.9f;
            this.graphic = (Sprite)spriteMap;

            this.center = new Vec2(16f, 23f);
            this.collisionSize = new Vec2(16f, 16f);
            this.collisionOffset = new Vec2(-8f, -16f);
            this._visibleInGame = false;
        }
        public void SpawnDuck(Profile pro)
        {
            if(!_duckSpawned)
            {
                _duckSpawned = true;
                pro.duck.position = position;
            }
        }
    }
}