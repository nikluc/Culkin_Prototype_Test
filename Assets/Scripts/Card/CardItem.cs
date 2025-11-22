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
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _cardAnimationController = GetComponent<CardAnimationController>();
        CardSetup();
    }

    public void CardSetup()
    {
        _cardAnimationController.icon.GetComponent<Image>().sprite = cardData.cardSprite;
        gameObject.name = cardData.name;
        _cardAnimationController.icon.SetActive(false);
        ResetCard();
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

        if (isFliped) return;

    
            isFliped = true;

            _cardAnimationController.FlipFaceUp(() =>
            {

                //testing to see if the flip back was auto triggered
                if(!autoTriggered)
                {
                    // Notify GameManager that this card is flipped and ready for matching check

                    FlipBack();
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
