using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    public Transform Plate;
    public Transform Rot;

    public Transform CameraPoint;
    public Material TargetMat;

    [Range(0, 1000)]
    public float HSpeed = 100f;

    [Range(0, 1000)]
    public float VSpeed = 100f;
    [Range(0.1f, 20)]
    public float DrawPointSize = 5f;

    private float totalRotateV;
    private LineRenderer lineRender;

    private Texture2D targetTex;

    #region UI
    public Text TxtH;
    public Text TxtV;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        lineRender = GetComponent<LineRenderer>();
        targetTex = new Texture2D(512, 512);
        for (var i = 0; i < 512; i++)
        {
            for (var j = 0; j < 512; j++)
            {
                targetTex.SetPixel(i, j, Color.blue);
            }
        }
        targetTex.Apply();
        TargetMat.mainTexture = targetTex;
        //targetTex = TargetMat.mainTexture as Texture2D;
    }

    public void SliderUpdate(Slider sender)
    {
        if (sender.name == "SliderVSpeed")
        {
            TxtV.text = $"VSpeed:{sender.value}";
            VSpeed = sender.value;
        }
        else if (sender.name == "SliderHSpeed")
        {
            TxtH.text = $"HSpeed:{sender.value}";
            HSpeed = sender.value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Plate.Rotate(Vector3.up * HSpeed * Time.deltaTime);

        totalRotateV += VSpeed * Time.deltaTime;
        totalRotateV %= 360;
        if (totalRotateV >= 180)
        {
            Rot.Rotate(Vector3.forward * VSpeed * Time.deltaTime * -1);
        }
        else
        {
            Rot.Rotate(Vector3.forward * VSpeed * Time.deltaTime);
        }
        //Debug.DrawLine(CameraPoint.position, transform.position, Color.red, 5f);

        lineRender.positionCount = 2;
        lineRender.SetPositions(new[] { CameraPoint.position, transform.position });
        var ray = new Ray(CameraPoint.position, transform.position - CameraPoint.position);
        //Debug.DrawRay(ray, Color.gray);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {
            var r = hitInfo.collider.GetComponent<MeshRenderer>();

            var meshCollider = hitInfo.collider as MeshCollider;

            if (r == null || r.material == null ||
                r.sharedMaterial == null || meshCollider == null)
                return;

            Vector2 targetVec = new Vector2(hitInfo.textureCoord.x * targetTex.width, hitInfo.textureCoord.y * targetTex.height);
            for (var i = 0; i < targetTex.width; i++)
            {
                for (var j = 0; j < targetTex.height; j++)
                {
                    if (Vector2.Distance(targetVec, new Vector2(i, j)) < DrawPointSize)
                    {
                        targetTex.SetPixel(i, j, Color.red);
                    }
                }
            }
            targetTex.Apply();
            TargetMat.mainTexture = targetTex;
        }
    }
}
