using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UITest : MonoBehaviour
{
    //Singleton.
    private static UITest _instance;
    public static UITest Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UITest>();
            }

            return _instance;
        }
    }

    [SerializeField] private InventoryUISlot InventorySlotPrefab;
    private InventoryUISlot[,] inventoryTable;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void GenerateUIWindowTable(Vector2 startingPos,int numColumns, int numRows, int windowWidth, int windowHeight)
    {
        inventoryTable = new InventoryUISlot[numRows,numColumns];
        Vector2 posToPlaceUIWindow = startingPos;

        for (int r = 0; r < inventoryTable.GetLength(0); r++)
        {
            for (int c = 0; c < inventoryTable.GetLength(1); c++)
            {
                InventoryUISlot inventorySlot = Instantiate(InventorySlotPrefab, posToPlaceUIWindow, Quaternion.identity);
                posToPlaceUIWindow.x += windowWidth;

                inventoryTable[r, c] = inventorySlot;
                inventorySlot.SetSize(windowWidth, windowHeight);


                //Set Colour.
                //CanvasRenderer canvasRendererOfWindow = inventorySlot.GetComponent<CanvasRenderer>();

                //Color colourOfUIWindow = (c + r) % 2 == 0 ? Color.grey : Color.black;
                //canvasRendererOfWindow.SetColor(colourOfUIWindow);
            }

            posToPlaceUIWindow = new Vector2(startingPos.x, startingPos.y - (windowHeight * (r + 1)));
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            GenerateUIWindowTable(Input.mousePosition, 5, 5, 110, 110);


            //UIWindow uIWindow = Instantiate(UIWindowPrefab, Input.mousePosition, Quaternion.identity);

            //uIWindow.SetSize(50, 50);
            //spawn UI Window.

        }
    }
}
