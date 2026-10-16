using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TiledCS;
using System.Collections.Generic;
using CollisionExample.Collisions;

namespace game_project_3;

public class Tilemap
{
    private readonly string _mapFile;
    private readonly string _textureName;

    private TiledMap _map;
    private TiledTileset _tileset;
    private Texture2D _texture;

    public int WidthInPixels => _map.Width * _map.TileWidth;
    public int HeightInPixels => _map.Height * _map.TileHeight;

    /// <param name="mapFile">Path to the TMX, e.g. "Content/test.tmx"</param>
    /// <param name="textureName">Content pipeline asset name of the tileset PNG, e.g. "tileset"</param>
    public Tilemap(string mapFile, string textureName)
    {
        _mapFile = mapFile;
        _textureName = textureName;
    }

    public void LoadContent(ContentManager content)
    {
        _map = new TiledMap(_mapFile);

        string tsxPath = Path.Combine(Path.GetDirectoryName(_mapFile), _map.Tilesets[0].source);
        _tileset = new TiledTileset(tsxPath);

        _texture = content.Load<Texture2D>(_textureName);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var layer in _map.Layers)
        {
            if (layer.type != TiledLayerType.TileLayer) continue;

            for (int i = 0; i < layer.data.Length; i++)
            {
                int gid = layer.data[i];
                if (gid == 0) continue;

                var mapTileset = _map.GetTiledMapTileset(gid);
                var rect = _map.GetSourceRect(mapTileset, _tileset, gid);

                var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                var position = new Vector2(
                    (i % layer.width) * _map.TileWidth,
                    (i / layer.width) * _map.TileHeight);

                spriteBatch.Draw(_texture, position, source, Color.White);
            }
        }
    }

    public List<BoudningRectangle> GetSolidRectangles(string layerName)
    {
        var result = new List<BoudningRectangle>();

        foreach (var layer in _map.Layers)
        {
            if (layer.type != TiledLayerType.ObjectLayer || layer.name != layerName) continue; //checking for object layers and correct layer name, if not then skips layer

            foreach (var obj in layer.objects)
            {
                if (obj.properties == null) continue;

                foreach (var prop in obj.properties)
                {
                    if (prop.name == "solid" && prop.value == "true")
                    {
                        result.Add(new BoudningRectangle(obj.x, obj.y, obj.width, obj.height));
                    }
                }
            }
        }

        return result;
    }


    public List<Vector2> GetPoints(string layerName, string objectName)
    {
        var result = new List<Vector2>();

        foreach (var layer in _map.Layers)
        {
           if (layer.type != TiledLayerType.ObjectLayer || layer.name != layerName) continue; //checking for object layers and correct layer name, if not then skips layer 

           foreach (var obj in layer.objects)
            {
                if (obj.name != objectName) continue;

                result.Add(new Vector2(obj.x, obj.y));
            }
        }

        return result;
    }
}