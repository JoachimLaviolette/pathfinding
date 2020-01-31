using UnityEngine;
using System;
using JoaDev.Utils;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          Grid class
 * ------------------------------------------------
 */
namespace Algo_0
{
    public class Grid<T>
    {
        public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
        public class OnGridValueChangedEventArgs : EventArgs { public int x; public int y; }

        private int width;
        private int height;
        private float cellSize;
        private float borderSize;
        private Vector3 originPosition; // Used to position the grid with an offset

        private T[,] elements;
        private TextMesh[,] elementsValues;
        private GameObject[,] cellQuads;
        private GameObject cellBorderQuad;

        private static bool IS_DEBUG = false;
        private static Color CELL_COLOR = Utils.GetColorFromString("9c9c9c");
        private static Color BORDER_COLOR = Utils.GetColorFromString("AFAFAF");

        public static int CELL_BACKGROUND_DEPTH = 10;
        public static int CELL_BORDER_DEPTH = 15;

        public Grid(int width, int height, float cellSize, float borderSize, Vector3 originPosition, Func<int, int, Grid<T>, T> createGridElementFunc)
        {
            OnGridValueChanged = null;

            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.borderSize = borderSize;
            this.originPosition = originPosition;
            this.elements = new T[this.width, this.height];

            this.InitializeElements(createGridElementFunc);
            this.Draw();
        }

        public int GetWidth() { return this.width; }
        public int GetHeight() { return this.height; }
        public float GetCellSize() { return this.cellSize; }
        public float GetBorderSize() { return this.borderSize; }
        public Vector3 GetOriginPosition() { return this.originPosition; }
        public Vector3 GetCenteredPosition() { return (this.originPosition + new Vector3(this.width, this.height) * cellSize) / 2; }
        public Vector3 GetWorldPosition(int x, int y, int z = 0)
        {
            return new Vector3(x, y, z) * this.cellSize + this.originPosition;
        }

        public Vector3 GetWorldPosition(float x, float y, float z = 0f)
        {
            return new Vector3(x, y, z) * this.cellSize + this.originPosition;
        }

        /**
         * Get x and y float values from a Vector3 position
         */
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - this.originPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - this.originPosition).y / cellSize);
        }

        /**
         * Set the element at the specified index
         */
        public void SetElement(int x, int y, T element)
        {
            if (x >= 0 && x < this.width
                && y >= 0 && y < this.height)
            {
                this.elements[x, y] = element;
                OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
            }
        }

        /**
         * Set the element at the specified index using a Vector3 position
         */
        public void SetElement(Vector3 worldPosition, T element)
        {
            int x, y;
            this.GetXY(worldPosition, out x, out y);
            this.SetElement(x, y, element);
        }

        /**
         * Get the element at the specified index
         */
        public T GetElement(int x, int y)
        {
            if (x >= 0 && x < this.width && y >= 0 && y < this.height) return this.elements[x, y];
            return default(T);
        }

        /**
         * Get the element at the specified index using a Vector3 position
         */
        public T GetElement(Vector3 worldPosition)
        {
            int x, y;
            this.GetXY(worldPosition, out x, out y);

            return this.GetElement(x, y);
        }

        /**
         * Initialize grid elements
         */
        private void InitializeElements(Func<int, int, Grid<T>, T> createGridElementFunc)
        {
            for (int x = 0; x < this.width; ++x)
            {
                for (int y = 0; y < this.height; ++y)
                {
                    this.elements[x, y] = createGridElementFunc(x, y, this);
                }
            }
        }

        /**
         * Draw the grid 
         */
        private void Draw()
        {
            this.cellQuads = new GameObject[this.width, this.height];

            for (int x = 0; x < this.width; ++x)
            {
                for (int y = 0; y < this.height; ++y)
                {
                    if (this.elements[x, y] is IDrawable) ((IDrawable)this.elements[x, y]).Draw();
                    else
                    {
                        // Draw cell's background
                        Vector3 worldPosition = this.GetWorldPosition(x, y);
                        Vector3 newWorldPosition = new Vector3(
                            worldPosition.x,
                            worldPosition.y,
                            CELL_BACKGROUND_DEPTH
                        );
                        this.cellQuads[x, y] = MeshUtils.CreateQuad(
                            this.cellSize - this.borderSize,
                            this.cellSize - this.borderSize,
                            null,
                            newWorldPosition,
                            CELL_COLOR,
                            "Cell: {" + x + "," + y + "}"
                        );
                    }
                }
            }

            this.cellBorderQuad = MeshUtils.CreateQuad(
                this.width * this.cellSize + this.borderSize,
                this.height * this.cellSize + borderSize,
                null,
                new Vector3(this.originPosition.x - this.borderSize, this.originPosition.y - this.borderSize, CELL_BORDER_DEPTH),
                BORDER_COLOR,
                "Borders"
            );
        }

        /**
         * Undraw all quads
         */
        private void UnDraw()
        {
            this.cellQuads = null;

            for (int x = 0; x < this.width; ++x)
            {
                for (int y = 0; y < this.height; ++y)
                {
                    if (this.elements[x, y] is IDrawable) ((IDrawable)this.elements[x, y]).UnDraw();
                    else GameObject.Destroy(this.cellQuads[x, y]);
                }
            }

            GameObject.Destroy(this.cellBorderQuad);
        }

        /**
         * Set debug mode
         */
        public void SetDebug(bool isEnabled)
        {
            IS_DEBUG = isEnabled;

            if (IS_DEBUG) this.DebugOn();
            else this.DebugOff();
        }

        /**
         * Return if debug mode is enabled
         */
        public bool IsDebugOn()
        {
            return IS_DEBUG;
        }

        /**
         * Show debug mode 
         */
        private void DebugOn()
        {
            this.UnDraw();

            this.elementsValues = new TextMesh[this.width, this.height];

            for (int x = 0; x < this.width; ++x)
            {
                for (int y = 0; y < this.height; ++y)
                {
                    this.elementsValues[x, y] = Utils.CreateWorldText(
                        this.elements[x, y].ToString(),
                        null,
                        this.GetWorldPosition(x, y) + new Vector3(this.cellSize, this.cellSize) * .5f,
                        22,
                        Color.white,
                        TextAnchor.MiddleCenter,
                        TextAlignment.Center,
                        0,
                        "CellText: {" + x + "," + y + "}"
                    );

                    //Debug.DrawLine(this.GetWorldPosition(x, y), this.GetWorldPosition(x, y + 1), Color.white, 100f);
                    //Debug.DrawLine(this.GetWorldPosition(x, y), this.GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }

            //Debug.DrawLine(this.GetWorldPosition(0, this.height), this.GetWorldPosition(this.width, this.height), Color.white, 100f);
            //Debug.DrawLine(this.GetWorldPosition(this.width, 0), this.GetWorldPosition(this.width, this.height), Color.white, 100f);

            OnGridValueChanged += this.UpdateElement;
        }

        /**
         * Hide debug mode
         */
        private void DebugOff()
        {
            for (int x = 0; x < this.width; ++x)
            {
                for (int y = 0; y < this.height; ++y)
                {
                    GameObject.Destroy(this.elementsValues[x, y]);
                }
            }

            this.elementsValues = null;
            OnGridValueChanged = null;

            this.Draw();
        }

        /**
         * Update the element at the args' x and y coordinates
         */
        private void UpdateElement(object sender, OnGridValueChangedEventArgs eventArgs)
        {
            this.elementsValues[eventArgs.x, eventArgs.y].text = this.elements[eventArgs.x, eventArgs.y].ToString();
        }
    }
}