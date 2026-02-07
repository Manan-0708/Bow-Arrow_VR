
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;   
public class radialselection : MonoBehaviour
{
    [Range(2,10)]
 
    public int numberofradialparts; 
    public GameObject radialpartprefab;
    public Transform radialpartcanvas;
    public float anglebetweenradialparts = 10;
    private List<GameObject> spawnedparts = new List<GameObject>();
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        spawnradialparts();
    }
    public void spawnradialparts()
    {
        foreach (var item in spawnedparts)
        {
            Destroy(item);
        }
        spawnedparts.Clear();
        for (int i = 0; i < numberofradialparts; i++)
        {
            float angle = i * 360 / numberofradialparts - anglebetweenradialparts/2;
            Vector3 radialparteulerangle = new Vector3(0, 0, angle);
            GameObject spawnradialpart = Instantiate(radialpartprefab, radialpartcanvas);
            spawnradialpart.transform.position = radialpartcanvas.position;
            spawnradialpart.transform.localEulerAngles = radialparteulerangle;
            spawnradialpart.GetComponent<Image>().fillAmount = (1 / (float)numberofradialparts) - (anglebetweenradialparts / 360);
            spawnedparts.Add(spawnradialpart);

        }
    }
}
