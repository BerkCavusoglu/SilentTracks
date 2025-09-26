using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    private AudioSource footstep_Sound;
    [SerializeField]
    private AudioClip[] footstep_Clip;

    private CharacterController character_Controller;

    [HideInInspector]
    public float volume_Min, volume_Max;
    private float accumulated_Distance;

    [HideInInspector]
    public float step_Distance;
    void Awake()
    {
        footstep_Sound = GetComponent<AudioSource>();
        character_Controller = GetComponentInParent<CharacterController>();
    }
    void Update()
    {
        CheckToPlayFootstepSound();
    }
    void CheckToPlayFootstepSound() 
    {
        // if we are not on the ground
        // Eðer karakter havadaysa, footstep çalma
        if (!character_Controller.isGrounded)
            return;

        // Aþaðýdaki kod sadece grounded ise çalýþýr
        if (character_Controller.velocity.sqrMagnitude > 0)
        {
            accumulated_Distance += Time.deltaTime;

            if (accumulated_Distance > step_Distance)
            {
                footstep_Sound.volume = Random.Range(volume_Min, volume_Max);
                footstep_Sound.clip = footstep_Clip[Random.Range(0, footstep_Clip.Length)];
                footstep_Sound.Play();
                accumulated_Distance = 0f;
            }
            else
            {
                accumulated_Distance = 0f;
            }
        }
    }
    }
 //class
