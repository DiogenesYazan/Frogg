using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SmoothCameraTransition : MonoBehaviour
{
    public Transform mainCameraPosition;
    public Transform configCameraPosition;
    public Transform playCameraPosition;
    public float transitionDuration = 1.5f;
    public float bobbingRotationAmount = 0.2f;
    public float bobbingSpeed = 0.5f;
    public Image fadeImage;

    private bool isTransitioning = false;
    private bool isPlayingTransition = false;
    private float transitionProgress = 0f;
    private Transform targetPosition;
    private Vector3 startPosition;
    private Quaternion fixedRotation; // Rotação fixa para o modo "Play"

    void Start()
    {
        transform.position = mainCameraPosition.position;
        transform.rotation = mainCameraPosition.rotation;

        if (fadeImage != null) fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void MoveToMain()
    {
        StartTransition(mainCameraPosition, false);
    }

    public void MoveToConfig()
    {
        StartTransition(configCameraPosition, false);
    }

    public void MoveToPlay()
    {
        StartTransition(playCameraPosition, true); // Indica que é a transição para "Play"
    }

    void StartTransition(Transform target, bool isPlay)
    {
        isTransitioning = true;
        isPlayingTransition = isPlay;
        targetPosition = target;
        startPosition = transform.position;
        transitionProgress = 0f;

        if (isPlayingTransition)
        {
            fixedRotation = transform.rotation; // Mantém a rotação atual para "Play"
        }
    }

    void Update()
    {
        // Aplica movimento suave de balanço na rotação
        ApplyBobbingEffect();

        if (isTransitioning)
        {
            transitionProgress += Time.deltaTime / transitionDuration;

            // Para transições de "Play", apenas movimenta a posição, sem alterar a rotação
            if (isPlayingTransition)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition.position, transitionProgress);
                transform.rotation = fixedRotation; // Mantém a rotação fixa durante o movimento "Play"

                if (fadeImage != null)
                {
                    fadeImage.color = new Color(0, 0, 0, transitionProgress);
                }
            }
            else
            {
                // Transição normal para outras posições (Main e Config)
                transform.position = Vector3.Lerp(startPosition, targetPosition.position, transitionProgress);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetPosition.rotation, transitionProgress);
            }

            if (transitionProgress >= 1f)
            {
                isTransitioning = false;

                if (isPlayingTransition)
                {
                    SceneManager.LoadScene("NomeDaCena"); // Substitua "NomeDaCena" pelo nome correto da cena
                }
            }
        }
    }

    void ApplyBobbingEffect()
    {
        float bobbingAngle = Mathf.Sin(Time.time * bobbingSpeed) * bobbingRotationAmount;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                                              transform.rotation.eulerAngles.y,
                                              bobbingAngle);
    }
}
