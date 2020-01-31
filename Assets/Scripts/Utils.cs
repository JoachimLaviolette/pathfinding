using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          Utils class
 * ------------------------------------------------
 */

namespace JoaDev.Utils
{
    public class Utils
    {
        /**
         * Return world space coordinates of the given position using the default scene camera
         */
        public static Vector3 GetWorldPosition(Vector3 position)
        {
            return Camera.main.ScreenToWorldPoint(position);
        }

        /**
         * Return world space coordinates of the given position using the given camera
         */
        public static Vector3 GetWorldPosition(Vector3 position, Camera camera)
        {
            return camera.ScreenToWorldPoint(position);
        }

        /**
         * Return screen space coordinates of the given position using the default scene camera
         */
        public static Vector3 GetScreenPosition(Vector3 position)
        {
            return Camera.main.WorldToScreenPoint(position);
        }

        /**
         * Return screen space coordinates of the given position using the given camera
         */
        public static Vector3 GetScreenPosition(Vector3 position, Camera camera)
        {
            return camera.WorldToScreenPoint(position);
        }

        /**
         * Return world space coordinates of the mouse position using the default scene camera
         */
        public static Vector3 GetMouseWorldPosition()
        {
            return GetWorldPosition(Input.mousePosition);
        }

        /**
         * Return the size of the sprite of the game object
         * In world space coordinates
         */
        public static Vector3 GetSpriteSize(GameObject gameObject)
        {
            Vector2 spriteSize = gameObject.GetComponent<SpriteRenderer>().sprite.rect.size;

            // Get the local sprite size
            Vector2 localSpriteSize = spriteSize / gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;

            // Convert it in world space size
            Vector3 worldSize = localSpriteSize;
            worldSize.x *= gameObject.transform.lossyScale.x;
            worldSize.y *= gameObject.transform.lossyScale.y;

            return worldSize;
        }

        /**
         * Return the Color element corresponding to the given hexadecimal string
         * If any error when trying to convert the html string to color
         * Return black color by default
         */
        public static Color GetColorFromString(string colorString)
        {
            Color color = Color.black; // set to black by default

            if (!colorString.Contains("#")) colorString = "#" + colorString;

            ColorUtility.TryParseHtmlString(colorString, out color);

            return color;
        }

        /**
         * Create the provided text at the specified world position
         */
        public static TextMesh CreateWorldText(
            string text, 
            Transform parent, 
            Vector3 localPosition, 
            int fontSize,
            Color fontColor, 
            TextAnchor anchor,
            TextAlignment alignment,
            int sortingOrder,
            string name = "WordTextGO"
        )
        {
            GameObject worldTextGO = new GameObject(name, typeof(TextMesh));
            
            Transform worldTextTransform = worldTextGO.transform;
            worldTextTransform.SetParent(parent, false);
            worldTextTransform.localPosition = new Vector3(localPosition.x, localPosition.y, 10);

            TextMesh worldTextTM = worldTextGO.GetComponent<TextMesh>();
            worldTextTM.anchor = anchor;
            worldTextTM.alignment = alignment;
            worldTextTM.text = text;
            worldTextTM.fontSize = fontSize;
            worldTextTM.fontStyle = FontStyle.Bold;
            worldTextTM.color = fontColor;
            worldTextTM.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return worldTextTM;
        }
    }

    public class MeshUtils
    {
        public static GameObject CreateQuad(
            float width,
            float height,
            Transform parent,
            Vector3 localPosition,
            Color color,
            string name = "QuadGO"
        )
        {
            GameObject quadGO = new GameObject(name);
            
            Transform quadTransform = quadGO.transform;
            quadTransform.SetParent(parent, false);
            quadTransform.localPosition = localPosition;
            
            MeshRenderer meshRenderer = quadGO.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
            meshRenderer.sharedMaterial.color = color == null ? Color.grey : color;

            MeshFilter meshFilter = quadGO.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();

            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, height, 0),
                new Vector3(width, height, 0)
            };
            mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;

            meshFilter.mesh = mesh;

            return quadGO;
        }
    }
}