using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using System.Text.RegularExpressions;

public class TransformInfo {
    public Vector3 position;
    public Quaternion rotation;
    public float scaleValue;
}

public class LSystemsGenerator : MonoBehaviour {
    public static int NUM_OF_TREES = 8;
    public static int MAX_ITERATIONS = 7;

    public int title = 1;
    public int iterations = 4;
    public float angle = 30f;

    public float maxBranchScaleValue = 0.1f;
    public float minBranchScaleValue = 0.05f;
    public float trunkScalueValue = 1f;

    public float jointSpace = 0.8f;

    public float variance = 10f;
    public int rule_number = 0;
    public float growing_cd = 0.2f;
    public bool hasTreeChanged = false;
    public GameObject Tree;
    public GameObject Sun;

    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject trunk;
    private Dictionary<char, string>[] rulesBook = new Dictionary<char, string>[50];


    private const string axiom = "X";
    private const int DAY_START = 6;
    private const int DAY_END = 18;

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];
    private int growing_index = 0;
    private float scaleInterval = 0;
    private float scaleValue = 0;
    private bool isTriggerGrow = false;
    private float grow_rate = 1.2f;

    void Start() {

        rulesBook[0] = new Dictionary<char, string>
        {
             //{ 'X', "[F+F]" },
            //{ 'X', "[F-[X+X]+F[+FX]-X]" },
             { 'X', "[F&+**[-F-XF-X][+F]**[-XF[+X]][^+F-X]" },
            { 'F', "FF" }
        };
        rulesBook[1] = new Dictionary<char, string>
        {
            //{ 'X', "[-FX][+FX][FX]" },
            { 'X', "[-FX][&FX][+FX][^FX][FX]" },
            { 'F', "FF" }
        };
        rulesBook[2] = new Dictionary<char, string>
        {
            { 'X', "[&FF[+XF-F+FX]--F+F-FX]****[&FF[+XF-F+FX]--F+F-FX]****[&FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };
        rulesBook[3] = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };
        rulesBook[4] = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };
        rulesBook[5] = new Dictionary<char, string>
        {
            { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };
        rulesBook[6] = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };
        rulesBook[7] = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };
        rulesBook[8] = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        for (int i = 0; i < randomRotationValues.Length; i++) {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        rules = rulesBook[rule_number];

        Generate();
        Grow();
    }

    void Update()
    {
        if(EnviroSkyMgr.instance.IsStarted())
            getEnviroVariables();
        if (growing_cd >= 0)
        {
            growing_cd -= Time.deltaTime;
        }
        Grow();
    }


    private void Generate() {
        //Destroy(Tree);

        Tree = Instantiate(treeParent);

        currentString = axiom;

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < iterations; i++) {
            foreach (char c in currentString) {
                sb.Append(rules.ContainsKey(c) ? rules[c] : c.ToString());
            }

            currentString = sb.ToString();
            sb = new StringBuilder();
        }

        scaleInterval = (maxBranchScaleValue - minBranchScaleValue) / Regex.Matches(currentString, "F").Count;
        scaleValue = maxBranchScaleValue - scaleInterval;

        if (trunk) {
            initialPosition = transform.position;
            GameObject truckInstance = Instantiate(trunk);
            truckInstance.transform.localScale = new Vector3(trunkScalueValue, trunkScalueValue, trunkScalueValue);
            Bounds bounds = truckInstance.GetComponentInChildren<MeshFilter>().mesh.bounds;
            transform.Translate(Vector3.up * bounds.size.y * trunkScalueValue * 2 / 3);

            truckInstance.transform.SetParent(Tree.transform);
            truckInstance.transform.position = initialPosition;
            truckInstance.transform.LookAt(transform.position);
        }

        Debug.Log(currentString);
    }

    private void Grow()
    {
        //for (int growing_index = 0; growing_index < currentString.Length; growing_index++)

        if (growing_index < currentString.Length && growing_cd < 0 && isTriggerGrow)
        {

            print("growing");
            switch (currentString[growing_index])
            {
                case 'F':
                    initialPosition = transform.position;
                    // Add leaf or branch
                    GameObject fLine = Instantiate(branch);
                    fLine.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                    Bounds bounds = fLine.GetComponentInChildren<MeshFilter>().mesh.bounds;
                    transform.Translate(Vector3.up * bounds.size.y * scaleValue * jointSpace);

                    // Initialize leaf or branch
                    fLine.transform.SetParent(Tree.transform);
                    // Vector3 centerPos = (transform.position + initialPosition) / 2;
                    fLine.transform.position = initialPosition;
                    fLine.transform.LookAt(transform.position);
                    scaleValue -= scaleInterval;

                    break;

                case 'X': // Do nothing for X, X is for string iterate update
                    break;

                case '+': // Rotate back along Z axis
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '-': // Rotate forward along Z axis
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '*': // Move up along Y axis
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '/': // Move down along Y axis
                    transform.Rotate(Vector3.down * 120 * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '&': // Rotate right along X axis
                    transform.Rotate(Vector3.right * angle * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '^': // Rotate left along X axis
                    transform.Rotate(Vector3.left * angle * (1 + variance / 100 * randomRotationValues[growing_index % randomRotationValues.Length]));
                    break;

                case '[': // Push tree state
                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation,
                        scaleValue = scaleValue
                    });
                    break;

                case ']': // Pop tree state
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    scaleValue = ti.scaleValue;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
            growing_cd = grow_rate;
            growing_index++;
        }
    }

    private void ResetRandomValues() {
        for (int i = 0; i < randomRotationValues.Length; i++) {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void getEnviroVariables()
    {
        // get variables from environment.
        //Sprint or summer, sunny, day time hours-> grow,  grow faster in summer
        string curSeason = EnviroSkyMgr.instance.Seasons.currentSeasons.ToString();
        int dayhours = EnviroSkyMgr.instance.Time.Hours;
        string curWeather = "Spring";
        if (EnviroSkyMgr.instance.GetCurrentWeatherPreset() != null)
            curWeather = EnviroSkyMgr.instance.Weather.currentActiveWeatherPreset.name;   

        if((curSeason == "Spring" || curSeason =="Summer") && (dayhours> DAY_START && dayhours<DAY_END) 
                && curWeather == "Clear Sky")
        {
            isTriggerGrow = true;
            if (curSeason == "Spring")
                grow_rate = 1.2f;
            if (curSeason =="Summer")
                grow_rate = 0.5f;                  
        }
        else
            isTriggerGrow = false;
    }
}