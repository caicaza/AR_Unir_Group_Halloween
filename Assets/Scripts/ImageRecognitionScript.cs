using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Para utilizar el AR
using UnityEngine.XR.ARFoundation;

public class ImageRecognitionScript : MonoBehaviour
{
    private ARTrackedImageManager _arTrackedImageManager;
    [SerializeField] private GameObject[] Objetos3D;

    private ARTrackedImageManager imageManager;
    private Dictionary<string, GameObject> objectos3dDictionary = new Dictionary<string, GameObject>();

    private string activeImage = "";
    private GameObject activeObject;
    //[SerializeField] private GameObject menuAR;
    // Para rotar el objeto rotz90
    private Quaternion rotObjectz = new Quaternion(0.707f, 0.0f, 0.0f, 0.707f);
    // Para rotar el objeto roty180
    private Quaternion rotObjecty = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);
    private Quaternion rotObject;

    private Vector3 positionObjetChange = new Vector3(0f, -0.1f, 0f);


    // [SerializeField] Canvas canvasAR_app;
    //[SerializeField] private Button canvasButton;

    private void Awake()
    {
        //Nos busca en la escena elementos de tipo ARTrackedImageManager
        _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
    }
    private void OnEnable()
    {
        //añadimos el imageChanged
        _arTrackedImageManager.trackedImagesChanged += onImageChanged;        
    }
    private void OnDisable()
    {
        //quitamos el imageChanged
        _arTrackedImageManager.trackedImagesChanged -= onImageChanged;
    }

    // Cada vez que cambie la imagen

    private void generateObject(string trackedImageName, Vector3 pos, Quaternion rot)
    {
        if (!this.activeImage.Equals(trackedImageName))
        {
            if (this.activeObject)
                destruirObjetos();

            this.activeObject = Instantiate(objectos3dDictionary[trackedImageName], pos, rot);
            this.activeImage = trackedImageName;
        }
        else
        {
            this.activeObject.transform.position = pos;
            this.activeObject.transform.rotation = rot;

        }
    }

    //DESTRUYE LOS OBJETOS 
    private void destruirObjetos()
    {
        this.activeImage = "";
        Destroy(this.activeObject);
        //rotateStatus = false;
    }
    private void destruirObjetos(string imageRemoved)
    {
        if (this.activeObject && imageRemoved == this.activeImage)
        {
            Destroy(this.activeObject);
            this.activeImage = "";
        }
    }
    // FUNCIÓN PARA CAMBIAR OBJETO DE ACUERDO A LA IMÁGEN
    public virtual void onImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // ARGS añadido
        foreach (var trackedImage in args.added)

        {
            this.generateObject(trackedImage.referenceImage.name,
                                trackedImage.transform.position + positionObjetChange,
                                (trackedImage.transform.rotation * this.rotObject));
        }

        // ARGS actualizado
        foreach (var trackedImage in args.updated)
        {
            if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                this.generateObject(trackedImage.referenceImage.name,
                    trackedImage.transform.position + positionObjetChange,
                    trackedImage.transform.rotation * this.rotObject);
            else if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.None)
            {
                this.destruirObjetos(trackedImage.referenceImage.name);
            }

        }

        //ARGS removed
        foreach (var trackedImage in args.removed)
        {
            this.destruirObjetos(trackedImage.referenceImage.name);
        }

    }

    // *** ROTAR OBJETO ACTIVO
    
    void Start()
    {
        rotObject = this.rotObjectz * this.rotObjecty;
        imageManager = this.GetComponent<ARTrackedImageManager>();

        for (int i = 0; i < Objetos3D.Length; i++)
        {
            objectos3dDictionary[Objetos3D[i].name] = Objetos3D[i];
        }
    }

    // Update is called once per frame
    void Update()
    {      

    }


}