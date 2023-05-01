using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingUI : MonoBehaviour
{
    [SerializeField] List<Image> _stars;
    [SerializeField] TextMeshProUGUI _ratingTmp;
    [SerializeField] TextMeshProUGUI _nameTmp;
    [SerializeField] Animator _animator;
    [SerializeField] Image _background;
    [SerializeField] Color _backgroundTextColor;
    private List<string> _positiveReviews = new List<string>() {
"Huzzah! My package hath arrived with haste!",
"Verily, this delivery service doth impress!",
"Prithee, use this delivery service for thy packages posthaste!",
"I am mightily pleased with this delivery service, it hath exceeded my expectations!",
"I hath received my package in good order and with great alacrity!",
"By my faith, this delivery service hath proved reliable and speedy!",
"I doth commend this delivery service for its promptness and care!",
"Well done, fair delivery service, my package hath arrived in excellent condition!",
"I shall speak praises of this delivery service to all who would listen!",
"This delivery service hath provided me with great satisfaction, I shall use it again!",
"I am thoroughly pleased with the service and swiftness of this delivery company!",
"In good faith, this delivery service hath surpassed all expectations!",
"By the holy grail, this delivery service is truly exceptional!",
"My package hath arrived with great speed and accuracy, a credit to this delivery service!",
"I cannot express my gratitude enough for this swift and dependable delivery service!",
"This delivery service hath earned my loyalty with its excellence and timeliness!",
"I hath received my package sooner than expected, a pleasant surprise from this delivery service!",
"By my troth, this delivery service hath won me over with its efficiency and reliability!",
"I shall recommend this delivery service to all in need of prompt and careful delivery!",
"This delivery service hath delivered my package with such haste, I am beyond pleased!"
    };
    private List<string> _negativeReviews = new List<string>() {
"Alas, my package hath been lost in transit!",
"This delivery service hath failed me in my hour of need!",
"By my oath, this delivery service hath proved woefully unreliable!",
"I am sorely disappointed in this delivery service's lack of punctuality!",
"By the saints, my package hath arrived in a sorry state!",
"I hath waited overlong for my package to arrive, to no avail!",
"This delivery service hath caused me great frustration and vexation!",
"I am displeased with the quality of service provided by this delivery company!",
"This delivery service hath shown great neglect in handling my package!",
"By my troth, this delivery service is in need of improvement!",
"My package hath been delayed without reason, a grave inconvenience!",
"I hath received my package damaged, a fault of this delivery service!",
"This delivery service hath failed to meet my expectations in every regard!",
"By my honour, this delivery service is not to be trusted!",
"I am sorely dissatisfied with the carelessness of this delivery service!",
"This delivery service hath caused me much frustration and distress!",
"I shall not use this delivery service again, for it hath failed me twice!",
"By the holy grail, this delivery service hath lost my package in the wilds!",
"This delivery service hath proven to be a hindrance rather than a help!",
"I am aggrieved by the lack of communication from this delivery service!"
    };
    private List<string> _names = new List<string>
    {
        "Gareth the Blacksmith",
"Eleanor the Weaver",
"Walter the Baker",
"Agatha the Apothecary",
"Rupert the Tanner",
"Helena the Innkeeper",
"Geoffrey the Tailor",
"Beatrice the Miller",
"Benedict the Scribe",
"Margery the Brewer",
"Giles the Fletcher",
"Petronilla the Chandlery",
"Simon the Potter",
"Isolde the Falconer",
"Balthazar the Merchant",
"Eudora the Embroiderer",
"Bernard the Butcher",
"Sybil the Grocer",
"Leofric the Carpenter",
"Ysabel the Candlemaker",
"Godfrey the Farrier",
"Rosamund the Cook",
"Anselm the Wainwright",
"Cecilia the Dyeworker",
"Hugh the Cooper",
"Avis the Woolspinner",
"Godric the Fisherman",
"Millicent the Cheesemaker",
"Emeric the Goldsmith",
"Winifred the Tiler",
"Edith the Bookbinder",
"Arnold the Clothier",
"Rowena the Fruiterer",
"Thibault the Armourer",
"Oriana the Embalmer",
"Gwendolyn the Candlemaker",
"Albert the Blacksmith",
"Griselda the Girdler",
"Gertrude the Glassblower",
"Marmaduke the Fletcher",
"Dorothea the Basketmaker",
"Lucius the Potter",
"Adelina the Vintner",
"Gawain the Shoemaker",
"Isadora the Locksmith",
"Fulke the Woodcarver",
"Clemence the Haberdasher",
"Rollo the Hay Merchant",
"Joan the Rope Maker",
"Marcella the Furrier",
"Thibault the Perfumer",
"Pernelle the Spindle Maker"
    };

    public void HideDescription()
    {
        _ratingTmp.gameObject.SetActive(false);
        _nameTmp.gameObject.SetActive(false);
    }
    public void PlayAnimation()
    {
        _animator.enabled = true;
    }
    public void ShowBackground()
    {
        _background.gameObject.SetActive(true);
        _ratingTmp.color = _backgroundTextColor;
        _nameTmp.color = _backgroundTextColor;

    }
    public void SetRating(float rating)
    {
        int i = 0;
        foreach(var star in _stars)
        {
            i++;
            if (i > rating) star.color = Color.black;
        }
        List<string> reviews = rating > 3 ? _positiveReviews : _negativeReviews;
        _ratingTmp.text = reviews[Random.Range(0, reviews.Count)];
        _nameTmp.text = _names[Random.Range(0, _names.Count)];
    }

    public IEnumerator DestroyAfter(float seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
