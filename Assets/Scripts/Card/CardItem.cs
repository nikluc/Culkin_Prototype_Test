using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CardItem : MonoBehaviour
{

    [Header("Refrences")]
    public CardData cardData;

    [Header("Properties")]
    public float revealDuration = 3f;
    private bool isFliped = false;

    private CanvasGroup _canvasGroup;
    private CardAnimationController _cardAnimationController;
    private GameManager gameManager;

    private void Awake()
    {
        if (_cardAnimationController == null)
        {
            _cardAnimationController = GetComponent<CardAnimationController>();
        }
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void CardSetup(bool skipreset = false)
    {
        Awake();
        _cardAnimationController.icon.GetComponent<Image>().sprite = cardData.cardSprite;
        gameObject.name = cardData.name;
        _cardAnimationController.icon.SetActive(false);
        if (!skipreset)
        {
            ResetCard();
        }
    }

    public bool IsFlipped()
    {
        return isFliped;
    }

    public bool isVanished()
    {
        return _canvasGroup ? !_canvasGroup.interactable : false;
    }

    public void ResetCard(bool forceReset = false)
    {
        _cardAnimationController.icon.SetActive(false);
        _cardAnimationController.icon.transform.localScale = Vector3.one;
        isFliped = false;
        SetCanvas(true);

        if (!forceReset)
        {
            // Start by showing card face to player
            StartCoroutine(RevealThenFlipBack());
        }
        else
        {
            StopCoroutine(RevealThenFlipBack());
        }
    }

    private IEnumerator RevealThenFlipBack()
    {
        // Flip card face up
        CardFlip(true);

        // Wait for 'revealDuration' seconds showing the face
        yield return new WaitForSeconds(revealDuration);

        // Flip card face down again
        FlipBack();
    }

    public void CardFlip(bool autoTriggered = false)
    {
        if (gameManager == null) return;
        if (isFliped) return;

    
            isFliped = true;

            _cardAnimationController.FlipFaceUp(() =>
            {
                if(!autoTriggered)
                {
                    // Notify GameManager that this card is flipped and ready for matching check
                    gameManager.OnCardFlipped(this);
                }
            });
        
    }

    public void FlipBack()
    {

        _cardAnimationController.FlipFaceDown(() => {
            _cardAnimationController.cardFace.transform.localScale = Vector3.one;
             _cardAnimationController.icon.SetActive(false);
            isFliped = false;
        });

    }

    public void Vanish()
    {
        // Simple vanish animation, disable gameobject after
        _cardAnimationController.Vanish(() =>
        {
            SetCanvas(false);
        });
    }

    void SetCanvas(bool interactable)
    {
        if (_canvasGroup == null)
            return;

        _canvasGroup.alpha = interactable ? 1 : 0;
        _canvasGroup.interactable = interactable;
    }

}
