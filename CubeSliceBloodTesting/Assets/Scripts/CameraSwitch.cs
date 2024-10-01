using UnityEngine;
using Cinemachine;  // Make sure to include the Cinemachine namespace

public class CameraSwitch : MonoBehaviour
{
    public CinemachineVirtualCamera firstPersonCam;  // Drag the First Person Cinemachine Virtual Camera here
    public CinemachineVirtualCamera thirdPersonCam;  // Drag the Third Person Cinemachine Virtual Camera here



    private bool isThirdPerson = false;  // Tracks whether third person is active

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))  // When 'Tab' is pressed, toggle the view
        {
            isThirdPerson = !isThirdPerson;  // Toggle between first and third person
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        if (isThirdPerson)
        {
            firstPersonCam.Priority = 0;  // Lower priority means this camera is deactivated
            thirdPersonCam.Priority = 10; // Higher priority activates this camera

        }
        else
        {
            firstPersonCam.Priority = 10;  // Activate the first person camera
            thirdPersonCam.Priority = 0;   // Deactivate the third person camera

        }
    }
}
