using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgo : MonoBehaviour
{
    public int nbStartIndi;
    public int nbStartRessource;
    public GameObject ressourcePrefab;
    public GameObject indiPrefab;
    public GameObject planeRef;

    private List<GameObject> spawnedRessources;
    private List<GameObject> mobs;
    private int nbMobsKeep = 4;
    private float timecount = 0;
    private int gen = 1;

    // Start is called before the first frame update
    void Start()
    {
        spawnedRessources = new List<GameObject>();
        mobs = new List<GameObject>();

        Init();
    }

    // Update is called once per frame
    void Update()
    {
        float moy = 0;
        for (int i = 0; i < mobs.Count; i++)
        {
            moy += mobs[i].GetComponent<Indi>().speed;
        }
        moy /= mobs.Count;
        Debug.Log(moy);
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 0.2f;
        }
        timecount += Time.deltaTime;
        if (spawnedRessources.Count == 0 || timecount >= 2) //quand on arrive à la fin du round
        {
            gen++;
            ChoseBestIndi(); //on garde le/les meilleurs
            Duplicate(); //On reproduit les meilleurs
            Init(); //on reset la scène avec le nouveaux individu et les ressources
            timecount = 0;
        }   
    }

    private void Duplicate()
    {
        while (mobs.Count < nbStartIndi)
        {
            int parentCount = mobs.Count;
            //on prend deux individus au hasard parmis les meilleurs
            GameObject parent1;
            GameObject parent2;
            int rdm1 = Random.Range(0, parentCount - 1);
            parent1 = mobs[rdm1];
            int rdm2 = Random.Range(0, parentCount - 1);
            while (rdm2 == rdm1)
            {
                rdm2 = Random.Range(0, parentCount - 1);
            }
            parent2 = mobs[rdm2];

            //on créer un mob  qui est un mix des deux en ajoutant des petits random
            float newSpeed = (parent1.GetComponent<Indi>().speed + parent2.GetComponent<Indi>().speed) / 2 + Random.Range(-3f, 3f); //calcul de la vitesse de l'enfant en fonction de la vitesse de ses parents et d'un random
            GameObject indi = Instantiate(indiPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            indi.GetComponent<Indi>().speed = newSpeed;
            indi.GetComponent<Indi>().senseRadius = 15;
            mobs.Add(indi);
        }
    }

    public void ChoseBestIndi()
    {
        mobs.Sort(SortByEaten);
        mobs.Reverse();


        while (mobs.Count > nbMobsKeep)
        {
            Destroy(mobs[nbMobsKeep]);
            mobs.RemoveAt(nbMobsKeep);
        }
    }

    static int SortByEaten(GameObject indi1, GameObject indi2)
    {
        return indi1.GetComponent<Indi>().GetEaten().CompareTo(indi2.GetComponent<Indi>().GetEaten());
    }

    public void Init()
    {
        //clear les anciennes ressources
        foreach (GameObject gameObject in spawnedRessources)
        {
            Destroy(gameObject);
        }
        spawnedRessources.Clear();



        float maxX = planeRef.GetComponent<MeshRenderer>().bounds.size.x / 2;
        float maxY = planeRef.GetComponent<MeshRenderer>().bounds.size.z / 2;

        //spawn des ressources
        for (int i = 0; i < nbStartRessource; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-maxX, maxX), 0.5f, Random.Range(-maxY, maxY));
            spawnedRessources.Add(Instantiate(ressourcePrefab, pos, Quaternion.identity));
        }
        //spawn des créatures
        if (gen == 1)
        {
            for (int i = 0; i < nbStartIndi; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxX, maxX), 0.5f, Random.Range(-maxY, maxY));
                GameObject indi = Instantiate(indiPrefab, pos, Quaternion.identity);
                indi.GetComponent<Indi>().speed = Random.Range(1, 20);
                indi.GetComponent<Indi>().senseRadius = 15;
                mobs.Add(indi);
            } 
        }
        else
        {
            for (int i = 0; i < mobs.Count; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-maxX, maxX), 0.5f, Random.Range(-maxY, maxY));
                mobs[i].transform.position = pos;
                mobs[i].GetComponent<Indi>().SetEaten(0);
            }
        }

    }
}
