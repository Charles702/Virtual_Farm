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
    public bool hasTreeChanged = false;
    public GameObject Tree = null;

    [SerializeField] private GameObject treeParent;
    [SerializeField] private GameObject branch;
    [SerializeField] private GameObject leaf;

    private const string axiom = "X";

    private Dictionary<char, string> rules;
    private Stack<TransformInfo> transformStack;
    private int titleLastFrame;
    private int iterationsLastFrame;
    private float angleLastFrame;
    private float widthLastFrame;
    private float lengthLastFrame;
    private string currentString = string.Empty;
    private Vector3 initialPosition = Vector3.zero;
    private float[] randomRotationValues = new float[100];

    private void Start() {
        titleLastFrame = title;
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;

        for (int i = 0; i < randomRotationValues.Length; i++) {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }

        transformStack = new Stack<TransformInfo>();

        rules = new Dictionary<char, string>
        {
            { 'X', "[&FF[+XF-F+FX]--F+F-FX]****[&FF[+XF-F+FX]--F+F-FX]****[&FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }


    private void Generate() {
        Destroy(Tree);

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

        for (int i = 0; i < currentString.Length; i++) {
            switch (currentString[i]) {
                case 'F':
                    initialPosition = transform.position;
                    transform.Translate(Vector3.up * length);
                    // Add leaf or branch
                    GameObject fLine = currentString[(i + 1) % currentString.Length] == 'X' || 
                                       currentString[(i + 3) % currentString.Length] == 'F' && 
                                       currentString[(i + 4) % currentString.Length] == 'X' ? 
                                       Instantiate(leaf) : Instantiate(branch);
                    // Initialize leaf or branch
                    fLine.transform.SetParent(Tree.transform);
                    fLine.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
                    fLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
                    fLine.GetComponent<LineRenderer>().startWidth = width;
                    fLine.GetComponent<LineRenderer>().endWidth = width;
                    break;
                
                case 'X': // Do nothing for X, X is for string iterate update
                    break;
                
                case '+': // Rotate back along Z axis
                    transform.Rotate(Vector3.back * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '-': // Rotate forward along Z axis
                    transform.Rotate(Vector3.forward * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '*': // Move up along Y axis
                    transform.Rotate(Vector3.up * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                 
                case '/': // Move down along Y axis
                    transform.Rotate(Vector3.down * 120 * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '&': // Rotate right along X axis
                    transform.Rotate(Vector3.right * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '^': // Rotate left along X axis
                    transform.Rotate(Vector3.left * angle * (1 + variance / 100 * randomRotationValues[i % randomRotationValues.Length]));
                    break;
                
                case '[': // Push tree state
                    transformStack.Push(new TransformInfo() {
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
        }

    }

    private void SelectTreeOne() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F-[X+X]+F[+FX]-X]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeTwo() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX][+FX][FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeThree() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[-FX]X[+FX][+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFour() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FF[+XF-F+FX]--F+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeFive() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[FX[+F[-FX]FX][-F-FXFX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSix() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[+FX][*+FX][/+FX]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeSeven() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[*+FX]X[+FX][/+F-FX]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void SelectTreeEight() {
        rules = new Dictionary<char, string>
        {
            { 'X', "[F[-X+F[+FX]][*-X+F[+FX]][/-X+F[+FX]-X]]" },
            { 'F', "FF" }
        };

        Generate();
    }

    private void ResetRandomValues() {
        for (int i = 0; i < randomRotationValues.Length; i++) {
            randomRotationValues[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

    private void ResetFlags() {
        iterationsLastFrame = iterations;
        angleLastFrame = angle;
        widthLastFrame = width;
        lengthLastFrame = length;
    }

    private void ResetTreeValues() {
        iterations = 4;
        angle = 30f;
        width = 1f;
        length = 2f;
        variance = 10f;
    }
}