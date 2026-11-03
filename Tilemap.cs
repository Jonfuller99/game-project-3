using System;
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
    private readonly string[] _textureNames;

    private TiledMap _map;

    // Keyed by each tileset's firstgid so we can look one up directly from a tile's gid
    private readonly Dictionary<int, TilesetData> _tilesets = new();

    private class TilesetData
    {
        public TiledTileset Tileset;
        public Texture2D Texture;
    }

    public int WidthInPixels => _map.Width * _map.TileWidth;
    public int HeightInPixels => _map.Height * _map.TileHeight;

    /// <param name="mapFile">Path to the TMX, e.g. "Content/test.tmx"</param>
    /// <param name="textureNames">
    /// Content pipeline asset names of the tileset PNGs, in the same order the
    /// tilesets appear in the TMX, e.g. "tileset", "props", "terrain"
    /// </param>
    public Tilemap(string mapFile, params string[] textureNames)
    {
        _mapFile = mapFile;
        _textureNames = textureNames;
    }

    public void LoadContent(ContentManager content)
    {
        _map = new TiledMap(_mapFile);

        if (_map.Tilesets.Length != _textureNames.Length)
        {
            throw new InvalidOperationException(
                $"Map '{_mapFile}' has {_map.Tilesets.Length} tileset(s) but " +
                $"{_textureNames.Length} texture name(s) were provided.");
        }

        string mapDir = Path.GetDirectoryName(_mapFile);

        for (int i = 0; i < _map.Tilesets.Length; i++)
        {
            var mapTileset = _map.Tilesets[i];
            string tsxPath = Path.Combine(mapDir, mapTileset.source);

            _tilesets[mapTileset.firstgid] = new TilesetData
            {
                Tileset = new TiledTileset(tsxPath),
                Texture = content.Load<Texture2D>(_textureNames[i])
            };
        }
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
                var data = _tilesets[mapTileset.firstgid];
                var rect = _map.GetSourceRect(mapTileset, data.Tileset, gid);

                var source = new Rectangle(rect.x, rect.y, rect.width, rect.height);
                var position = new Vector2(
                    (i % layer.width) * _map.TileWidth,
                    (i / layer.width) * _map.TileHeight);

                spriteBatch.Draw(data.Texture, position, source, Color.White);
            }
        }
    }

    public List<BoundingRectangle> GetSolidRectangles(string layerName)
    {
        var result = new List<BoundingRectangle>();

        foreach (var layer in _map.Layers)
        {
            if (layer.type != TiledLayerType.ObjectLayer || layer.name != layerName) continue;

            foreach (var obj in layer.objects)
            {
                if (obj.properties == null) continue;

                foreach (var prop in obj.properties)
                {
                    if (prop.name == "solid" && prop.value == "true")
                    {
                        result.Add(new BoundingRectangle(obj.x, obj.y, obj.width, obj.height));
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
            if (layer.type != TiledLayerType.ObjectLayer || layer.name != layerName) continue;

            foreach (var obj in layer.objects)
            {
                if (obj.name != objectName) continue;

                result.Add(new Vector2(obj.x, obj.y));
            }
        }

        return result;
    }



    /// <summary>
    /// Builds world-space collision rectangles from the collision objects defined
    /// on tiles in the TSX files. Pass a layer name to only use that tile layer.
    /// </summary>
    public List<BoundingRectangle> GetTileColliders(string layerName = null)
    {
        var result = new List<BoundingRectangle>();

        foreach (var layer in _map.Layers)
        {
            if (layer.type != TiledLayerType.TileLayer) continue;
            if (layerName != null && layer.name != layerName) continue;

            for (int i = 0; i < layer.data.Length; i++)
            {
                int gid = layer.data[i];
                if (gid == 0) continue;

                var mapTileset = _map.GetTiledMapTileset(gid);
                var data = _tilesets[mapTileset.firstgid];
                var tile = _map.GetTiledTile(mapTileset, data.Tileset, gid);

                if (tile?.objects == null) continue;

                float tileX = (i % layer.width) * _map.TileWidth;
                float tileY = (i / layer.width) * _map.TileHeight;

                foreach (var obj in tile.objects)
                {
                    // Tile collision objects are positioned relative to the tile's top-left corner
                    result.Add(new BoundingRectangle(
                        tileX + obj.x,
                        tileY + obj.y,
                        obj.width,
                        obj.height));
                }
            }
        }

        return result;
    }
}