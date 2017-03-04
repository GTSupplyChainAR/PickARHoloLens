using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpriteSheet : MonoBehaviour
{
    /// <summary> The number of columns of the texture </summary>
    [Tooltip("The number of columns of the texture")]
    public int _columns = 2;
    /// <summary> The number of rows of the texture </summary>
    [Tooltip("The number of rows of the texture")]
    public int _rows = 2;
    /// <summary> Scale the texture. This must be a non-zero number. negative scale flips the image </summary>
    [Tooltip("Scale the texture. This must be a non-zero number. negative scale flips the image")]
    public Vector2 _scale = new Vector3(1f, 1f);
    /// <summary> You can use this if you do not want the texture centered. (These are very small numbers .001) </summary>
    [Tooltip("You can use this if you do not want the texture centered. (These are very small numbers .001)")]
    public Vector2 _offset = Vector2.zero;
    /// <summary> You can use this to buffer frames to hide unwanted grid lines or artifacts </summary>
    [Tooltip("You can use this to buffer frames to hide unwanted grid lines or artifacts")]
    public Vector2 _buffer = Vector2.zero;
    /// <summary> Frames per second that you want to texture to play at </summary>
    [Tooltip("Frames per second that you want to texture to play at")]
    public float _framesPerSecond = 10f;
    /// <summary> Enable this if you want the animation to only play one time </summary>
    [Tooltip("Enable this if you want the animation to only play one time")]
    public bool _playOnce = false;
    /// <summary> Enable this if you want the texture to disable the renderer when it is finished playing </summary>
    [Tooltip("Enable this if you want the texture to disable the renderer when it is finished playing")]
    public bool _disableUponCompletion = false;
    /// <summary> Enable this if you want to register an event that fires when the animation is finished playing </summary>
    [Tooltip("Enable this if you want to register an event that fires when the animation is finished playing")]
    public bool _enableEvents = false;
    /// <summary> The animation will play when the object is enabled </summary>
    [Tooltip("The animation will play when the object is enabled")]
    public bool _playOnEnable = true;
    /// <summary> Set this to true if you want to create a new material instance </summary>
    [Tooltip("Set this to true if you want to create a new material instance")]
    public bool _newMaterialInstance = false;

    /// <summary> Keeps track of the current frame </summary>
    private int _index = 0;
    /// <summary> Keeps track of the texture scale  </summary>
    private Vector2 _textureSize = Vector2.zero;
    /// <summary> Material instance of the material we create (if needed) </summary>
    private Material _materialInstance = null;
    /// <summary> A flag so we know if we have a material instance we need to clean up (better than a null check i think) </summary>
    private bool _hasMaterialInstance = false;
    /// <summary> A flag to determine if the animation is currently playing </summary>
    private bool _isPlaying = false;

    /// <summary> The Event delegate </summary>
    public delegate void VoidEvent();
    /// <summary> A list of functions we need to call if events are enabled </summary>
    private List<VoidEvent> _voidEventCallbackList;

    /// <summary>
    /// Use this function to register your callback function with this script
    /// </summary>
    public void RegisterCallback(VoidEvent cbFunction)
    {
        // If events are enabled, add the callback function to the event list
        if (_enableEvents)
            _voidEventCallbackList.Add(cbFunction);
        else
            Debug.LogWarning("AnimateTiledTexture: You are attempting to register a callback but the events of this object are not enabled!");
    }

    /// <summary>
    /// Use this function to unregister a callback function with this script
    /// </summary>
    public void UnRegisterCallback(VoidEvent cbFunction)
    {
        // If events are enabled, unregister the callback function from the event list
        if (_enableEvents)
            _voidEventCallbackList.Remove(cbFunction);
        else
            Debug.LogWarning("AnimateTiledTexture: You are attempting to un-register a callback but the events of this object are not enabled!");
    }

    public void Play()
    {
        // If the animation is playing, stop it
        if (_isPlaying)
        {
            StopCoroutine("updateTiling");
            _isPlaying = false;
        }
        // Make sure the renderer is enabled
        GetComponent<Renderer>().enabled = true;

        //Because of the way textures calculate the y value, we need to start at the max y value
        _index = _columns;

        // Start the update tiling coroutine
        StartCoroutine(updateTiling());
    }

    public void ChangeMaterial(Material newMaterial, bool newInstance = false)
    {
        if (newInstance)
        {
            // First check our material instance, if we already have a material instance
            // and we want to create a new one, we need to clean up the old one
            if (_hasMaterialInstance)
                Object.Destroy(GetComponent<Renderer>().sharedMaterial);

            // create the new material
            _materialInstance = new Material(newMaterial);

            // Assign it to the renderer
            GetComponent<Renderer>().sharedMaterial = _materialInstance;

            // Set the flag
            _hasMaterialInstance = true;
        }
        else // if we dont have create a new instance, just assign the texture
            GetComponent<Renderer>().sharedMaterial = newMaterial;        

        // We need to recalc the texture size (since different material = possible different texture)
        CalcTextureSize();
    }

    private void Awake()
    {
        // Allocate memory for the events, if needed
        if (_enableEvents)
            _voidEventCallbackList = new List<VoidEvent>();

        //Create the material instance, if needed. else, just use this function to recalc the texture size
        ChangeMaterial(GetComponent<Renderer>().sharedMaterial, _newMaterialInstance);
    }

    private void OnDestroy()
    {
        // If we wanted new material instances, we need to destroy the material
        if (_hasMaterialInstance)
        {
            Object.Destroy(GetComponent<Renderer>().sharedMaterial);
            _hasMaterialInstance = false;
        }
    }

    // Handles all event triggers to callback functions
    private void HandleCallbacks(List<VoidEvent> cbList)
    {
        // For now simply loop through them all and call yet.
        for (int i = 0; i < cbList.Count; ++i)
            cbList[i]();
    }

    private void OnEnable()
    {

        CalcTextureSize();

        if (_playOnEnable)
            Play();
    }

    public void CalcTextureSize()
    {
        //set the tile size of the texture (in UV units), based on the rows and columns
        _textureSize = new Vector2(1f / _columns, 1f / _rows);

        // Add in the scale
        _textureSize.x = _textureSize.x / _scale.x;
        _textureSize.y = _textureSize.y / _scale.y;

        // Buffer some of the image out (removes gridlines and stufF)
        _textureSize -= _buffer;

        GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", _textureSize);
    }

    /// <summary>
    /// The main update function of this script
    /// </summary>
    private IEnumerator updateTiling()
    {
        _isPlaying = true;

        // This is the max number of frames
        int checkAgainst = (_rows * _columns);

        while (true)
        {
            // If we are at the last frame, we need to either loop or break out of the loop
            if (_index >= checkAgainst)
            {
                _index = 0;  // Reset the index

                // If we only want to play the texture one time
                if (_playOnce)
                {
                    if (checkAgainst == _columns)
                    {
                        // We are done with the coroutine. Fire the event, if needed
                        if(_enableEvents)
                            HandleCallbacks(_voidEventCallbackList);

                        if (_disableUponCompletion)
                            gameObject.GetComponent<Renderer>().enabled = false;

                        // turn off the isplaying flag
                        _isPlaying = false;

                        // Break out of the loop, we are finished
                        yield break;
                    }
                    else
                        checkAgainst = _columns;    // We need to loop through one more row
                }

            }

            // Apply the offset in order to move to the next frame
            ApplyOffset();

            //Increment the index
            _index++;

            // Wait a time before we move to the next frame. Note, this gives unexpected results on mobile devices
            yield return new WaitForSeconds(1f / _framesPerSecond);
        }        
    }

    private void ApplyOffset()
    {
        //split into x and y indexes. calculate the new offsets
        Vector2 offset = new Vector2((float)_index / _columns - (_index / _columns), //x index
            1 - ((_index / _columns) / (float)_rows));    //y index

        // Reset the y offset, if needed
        if (offset.y == 1)
            offset.y = 0.0f;

        // If we have scaled the texture, we need to reposition the texture to the center of the object
        offset.x += ((1f / _columns) - _textureSize.x) / 2.0f;
        offset.y += ((1f / _rows) - _textureSize.y) / 2.0f;

        // Add an additional offset if the user does not want the texture centered
        offset.x += _offset.x;
        offset.y += _offset.y;

        // Update the material
        GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}