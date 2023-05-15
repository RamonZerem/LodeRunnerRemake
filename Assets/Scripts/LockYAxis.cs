using UnityEngine;
using Cinemachine;
 
/// <summary>
/// An add-on module for Cinemachine Virtual Camera that locks the camera's Y co-ordinate
/// </summary>
[ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
public class LockYAxis : CinemachineExtension
{

    #region Inspector

    [SerializeField]
    private float mYPosition = 1.5f;
    
    #endregion

    #region CinemachineExtension

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            pos.y = mYPosition;
            state.RawPosition = pos;
        }
    }

    #endregion

}