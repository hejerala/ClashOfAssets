using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTotem : Building {

    //private Unit target;
    private float buffTimer = 0.0f;

    //public Vector3 bombOffset = new Vector3(0.0f, 4.0f, 0.0f);
    public Color deBuffColor = new Color(0.0f, 0.0f, 1.0f, 0.2f);
    public float deBuffInterval = 1.0f;
    public float deBuffRange = 15.0f;
    public float speedDebuff = 2.0f;
    //public Arrow arrowPrefab;

    private bool hasDrawnRange = false;

    // Use this for initialization
    //void Start () { }

    // Update is called once per frame
    void Update() {
        if (!GameMode.isBuilding && !hasDrawnRange)
            DrawRange();
        //if (target == null)
        //    LookForTarget();
        buffTimer += Time.deltaTime;
        if (buffTimer >= deBuffInterval) {
            LookForTargets();
            buffTimer = 0.0f;
        }
    }

    void DrawRange() {
        GameObject buffRangeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        buffRangeSphere.transform.position = transform.position;
        buffRangeSphere.transform.localScale = new Vector3(deBuffRange, deBuffRange, deBuffRange);
        Material sphereMat = buffRangeSphere.GetComponent<Renderer>().material;
        sphereMat.color = deBuffColor;
        //Changes the material rendering mode to Fade (so the transparency shows)
        sphereMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        sphereMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        sphereMat.SetInt("_ZWrite", 0);
        sphereMat.DisableKeyword("_ALPHATEST_ON");
        sphereMat.EnableKeyword("_ALPHABLEND_ON");
        sphereMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        sphereMat.renderQueue = 3000;
        buffRangeSphere.transform.SetParent(transform);
        hasDrawnRange = true;
    }

    void LookForTargets() {
        Collider[] surroundingColliders = Physics.OverlapSphere(transform.position, deBuffRange);
        foreach (Collider c in surroundingColliders) {
            Unit unit = c.GetComponent<Unit>();
            if (unit != null) {
                unit.OnDebuffSpeed(speedDebuff, deBuffInterval); ;
            }
        }
    }

}
