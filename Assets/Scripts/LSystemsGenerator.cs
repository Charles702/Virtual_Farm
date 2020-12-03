using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;
using UnityEngine.UIElements;

public class TransformInfo {
    public Vector3 position;
    public Quaternion rotation;
}

public class LSystemsGenerator : MonoBehaviour {
    public static int NUM_OF_TREES = 8;
    public static int MAX_ITERATIONS = 7;

    public int title = 1;
    public int iterations = 4;
    public float angle = 30f;
    public float width = 0.01f;
    public float length = 2f;
    public float variance = 10f;
    public int rule_number = 0;
    public float growing_cd = 0.2f;
    public bool hasTreeChanged = false;
    public GameObject Tree;
    public GameObject Sun;

    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;
    private Dictionary<char, string>[] rulesBook = new Dictionary<char, string>[50];


    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];
    private int growing_index = 0;

    void Start() {

        rulesBook[0] = new Dictionary<char, string>
        {
            //{ 'X', "[F-]" },
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            //{ 'X', "[F-F][" },
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
    }

    void Update()
    {
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

        Debug.Log(currentString);
    }

    private void Grow()
    {
        if (growing_index < currentString.Length && growing_cd < 0 && Sun.transform.position.y >= 0f)
        {
            print("growing");
            switch (currentString[growing_index])
            {
                case 'F':
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);
                    // Add leaf or branch
                    GameObject fLine = currentString[(growing_index + 1) % currentString.Length] == 'X' ||
                                       currentString[(growing_index + 3) % currentString.Length] == 'F' &&
                                       currentString[(growing_index + 4) % currentString.Length] == 'X' ?
                                       Instantiate(leaf) : Instantiate(branch);
                    // Initialize leaf or branch
                    fLine.transform.SetParent(Tree.transform);
                    Vector3 centerPos = (transform.position + initialPosition) / 2;
                    fLine.transform.position = centerPos;
                    Vector3 cylinder_up = fLine.transform.Find("Top").transform.position - fLine.transform.Find("Bot").transform.position;
                    float fLength = cylinder_up.magnitude;
                    print("fLength: " + fLength);
                    print("length: " + length);
                    float r = length / fLength;
                    fLine.transform.localScale = new Vector3(0.5f, 1.2f, 0.5f) * r;
                    Vector3 to = transform.position - centerPos;

                    fLine.transform.rotation = transform.rotation;
                    //RotateAround(fLine.transform.Find("Bot").transform.position, Vector3.right, 30f);

                    //fLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    //fLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    //fLine.GetComponent<LineRenderer>().startWidth = width;
                    //fLine.GetComponent<LineRenderer>().endWidth = width;
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
                    print("[ position: " + transform.position + " rotation: " + transform.rotation);

                    transformStack.Push(new TransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;

                case ']': // Pop tree state
                    TransformInfo ti = transformStack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;

                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
            growing_cd = 0.01f;
            growing_index++;
        }
    }

    private void ResetRandomValues() {
        for (int i = 0; i < randomRotationValues.Length; i++) {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetTreeValues() {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 10f;
    }
}