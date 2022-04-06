using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Listened to <see href="https://youtu.be/iPkASX1nKTs">this</see> constantly while I coded this in under 48 hours.
/// <br></br>
/// This uses the Bankers class to process everything.
/// </summary>
/// 
/// <author>
/// Lucas Rendell
/// </author>
public class UserInterfaceManager : MonoBehaviour
{
    // Consts
    private const string FILE_PATH = "/Input/";
    private const string MAX_FILE_NAME = "max.txt";
    private const string ALLOCATED_FILE_NAME = "alloc.txt";

    // Externals
    [SerializeField] private GridLayoutGroup allocationGridLayout, maxGridLayout, availableGridLayout, needGridLayout;
    [SerializeField] private GameObject inputFieldEditablePrefab;
    [SerializeField] private GameObject inputFieldNonEditablePrefab;
    [SerializeField] private GameObject infoTextPrefab;
    [SerializeField] private Transform infoTextsParent;
    [SerializeField] private InputField inputFieldRows;
    [SerializeField] private InputField inputFieldColumns;

    // Internals
    private Bankers bankers;
    private int rows = 4, columns = 4;
    private List<InputField> allocationMatrix, maxMatrix, availableMatrix, needMatrix;    // Disclaimer: I'm not truly object pooling, don't get fooled
    private List<GameObject> infoTexts;

    #region Unity Methods

    private void Awake()
    {
        // For bug-fix reasons, we initialize the Lists.
        allocationMatrix = new List<InputField>();
        maxMatrix = new List<InputField>();
        availableMatrix = new List<InputField>();
        needMatrix = new List<InputField>();
        infoTexts = new List<GameObject>();
        UpdateMatrixSizes();
    }

    #endregion

    #region Public Methods

    public void FillFromInputFile()
    {
        StreamReader streamReader = new StreamReader(Application.dataPath + FILE_PATH + MAX_FILE_NAME);
        List<int> max = new List<int>();
        int columns = 0;
        int rows = 0;
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            rows++;
            columns = 0; // lol
            foreach (string s in line.Split(','))
            {
                max.Add(int.Parse(s));
                columns++;
            }
        }
        List<int> alloc = new List<int>();
        streamReader.Close();
        streamReader = new StreamReader(Application.dataPath + FILE_PATH + ALLOCATED_FILE_NAME);
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine();
            foreach (var s in line.Split(','))
            {
                alloc.Add(int.Parse(s));
            }
        }
        streamReader.Close();

        PopulateMatrices(columns, rows, max, alloc);
    }

    /// <summary>
    /// Hard-coded
    /// </summary>
    public void FillFromPreset(bool preset1)
    {
        if (preset1)
        {
            PopulateMatrices(4, 5, 
                new List<int> { 0, 0, 1, 2, 1, 7, 5, 0, 2, 3, 5, 6, 0, 6, 5, 2, 0, 6, 5, 6 },
                new List<int> { 0, 0, 1, 2, 1, 0, 0, 0, 1, 3, 5, 4, 0, 6, 3, 2, 0, 0, 1, 4 },
                new List<int> { 1, 5, 2, 0 });
        }
        else
        {
            List<int> random1 = new List<int>(25);
            List<int> random2 = new List<int>(25);
            List<int> random3 = new List<int>(5);
            for (int i = 0; i < 25; i++)
            {
                random1.Add((int)Random.Range(3, 10));
                random2.Add((int)Random.Range(0, 6));
            }
            for (int i = 0; i < 5; i++)
            {
                random3.Add((int)Random.Range(0, 10));
            }
            PopulateMatrices(5, 5,
                random1,
                random2,
                random3);
        }
    }

    public void OpenInputFolder()
    {
        try
        {
            Application.OpenURL((Application.dataPath) + FILE_PATH);
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    public void UpdateRowAmount()
    {
        rows = InputFieldNum(inputFieldRows);
        UpdateMatrixSizes();
    }

    public void UpdateColumnAmount()
    {
        columns = InputFieldNum(inputFieldColumns);
        UpdateMatrixSizes();
    }

    /// <summary>
    /// This method is super ugly, don't pay attention to it
    /// </summary>
    public void DoBankers()
    {
        List<InputField> _allocationMatrix = new List<InputField>(allocationMatrix), _maxMatrix = new List<InputField>(maxMatrix), _availableMatrix = new List<InputField>(availableMatrix);
        int[] available = new int[_availableMatrix.Count];
        for (int i = 0; i < _availableMatrix.Count; i++)
        {
            available[i] = InputFieldNum(_availableMatrix[i]);
            Debug.Log(available[i]);
        }
        int[,] maximum = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                maximum[i, j] = InputFieldNum(_maxMatrix[0]);
                _maxMatrix.RemoveAt(0);
            }
        }
        int[,] allocation = new int[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                allocation[i, j] = InputFieldNum(_allocationMatrix[0]);
                _allocationMatrix.RemoveAt(0);
            }
        }
        Bankers bankers = new Bankers(available, maximum, allocation, rows, columns);
        List<string> output = bankers.DoEverything();
        List<int> need = bankers.GetNeedMatrix();
        DisplayOutput(output);
        PopulateNeedMatrix(need);
    }

    #endregion

    private void PopulateMatrices(int columns, int rows, List<int> maxData, List<int> allocData)
    {
        inputFieldRows.text = rows.ToString();
        inputFieldColumns.text = columns.ToString();
        this.rows = rows;
        this.columns = columns;
        UpdateMatrixSizes();
        for (int i = 0; i < maxData.Count; i++)
        {
            maxMatrix[i].text = maxData[i].ToString();
        }
        for (int i = 0; i < allocData.Count; i++)
        {
            allocationMatrix[i].text = allocData[i].ToString();
        }
    }
    private void PopulateMatrices(int columns, int rows, List<int> maxData, List<int> allocData, List<int> availableData)
    {
        PopulateMatrices(columns, rows, maxData, allocData);
        for (int i = 0; i < availableData.Count; i++)
        {
            availableMatrix[i].text = availableData[i].ToString();
        }
    }

    private void PopulateNeedMatrix(List<int> needData)
    {
        int i = 0;
        foreach (InputField input in needMatrix)
        {
            input.text = needData[i].ToString();
            i++;
        }
    }

    private void DisplayOutput(List<string> output)
    {
        foreach (GameObject gameObject in infoTexts)
        {
            Destroy(gameObject);
        }
        infoTexts = new List<GameObject>();
        foreach (string s in output)
        {
            GameObject temp = Instantiate(infoTextPrefab, infoTextsParent);
            infoTexts.Add(temp);
            temp.GetComponent<Text>().text = s;
        }
    }

    private void UpdateMatrixSizes()
    {
        ClearMatrix();

        allocationGridLayout.constraintCount = columns;
        maxGridLayout.constraintCount = columns;
        needGridLayout.constraintCount = columns;
        availableGridLayout.constraintCount = columns;
        allocationMatrix = new List<InputField>();
        maxMatrix = new List<InputField>();
        availableMatrix = new List<InputField>();
        needMatrix = new List<InputField>();
        int size = (rows * columns);
        for (int i = 0; i < size; i++)
        {
            allocationMatrix.Add(Instantiate(inputFieldEditablePrefab, allocationGridLayout.transform).GetComponent<InputField>());
            maxMatrix.Add(Instantiate(inputFieldEditablePrefab, maxGridLayout.transform).GetComponent<InputField>());
            needMatrix.Add(Instantiate(inputFieldNonEditablePrefab, needGridLayout.transform).GetComponent<InputField>());
        }
        for (int i = 0; i < columns; i++)
        {
            availableMatrix.Add(Instantiate(inputFieldEditablePrefab, availableGridLayout.transform).GetComponent<InputField>());
        }
        foreach (InputField input in allocationMatrix)
        {
            input.text = "0";
        }
        foreach (InputField input in maxMatrix)
        {
            input.text = "0";
        }
        foreach (InputField input in availableMatrix)
        {
            input.text = "0";
        }
        foreach (InputField input in needMatrix)
        {
            input.text = "_";
        }
    }

    private void ClearMatrix()
    {
        foreach (InputField input in allocationMatrix)
        {
            Destroy(input.gameObject);
        }
        foreach (InputField input in maxMatrix)
        {
            Destroy(input.gameObject);
        }
        foreach (InputField input in availableMatrix)
        {
            Destroy(input.gameObject);
        }
        foreach (InputField input in needMatrix)
        {
            Destroy(input.gameObject);
        }
    }

    private int InputFieldNum(InputField inputf)
    {
        try
        {
            return int.Parse(inputf.text);
        }
        catch (System.Exception)
        {
            Debug.Log("Ran into an error.");
            return 0;
        }
    }
}
