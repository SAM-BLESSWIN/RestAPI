using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using SimpleJSON;

public class PokeCard
{
    public string pokemonImageURL;
    public string pokemonID;
    public string pokemonName;
    public string[] pokemonTypes=new string[2];
}

public class PokeAPIController : MonoBehaviour
{
    [SerializeField] private RawImage PokeImage;
    [SerializeField] private TMP_Text PokeID;
    [SerializeField] private TMP_Text PokeName;
    [SerializeField] private TMP_Text[] PokeTypes;
    [SerializeField] private TMP_InputField SearchID;
    [SerializeField] private Button Searchbtn;
    [SerializeField] private Button RandomCard;
    [SerializeField] private TMP_Text Error;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    private void OnEnable()
    {
        RandomCard.onClick.AddListener(() =>
        {
            GetRandomPokemon();
        });

        Searchbtn.onClick.AddListener(() =>
        {
            SearchPokemon();
        });
    }

    private void Reset()
    {
        Error.gameObject.SetActive(false);
        PokeImage.texture = Texture2D.blackTexture;
        PokeID.text = "#";
        PokeName.text = "Loading...";

        foreach (TMP_Text type in PokeTypes)
        {
            type.text = "";
        }
    }

    private void GetRandomPokemon()
    {
        Reset();
        int randompokeID = Random.Range(1, 808);
        PokeID.text = "#" + randompokeID;
        StartCoroutine(GetPokemonAtIndex(randompokeID));
    }

    private void SearchPokemon()
    {
        Reset();
        int pokeID = int.Parse(SearchID.text);
        if (SearchID.text!="" && (pokeID > 0 && pokeID < 808))
        {
            PokeID.text = "#" + pokeID;
            StartCoroutine(GetPokemonAtIndex(pokeID));
        }
        else
        {
            Error.gameObject.SetActive(true);
        }
    }

    IEnumerator GetPokemonAtIndex(int pokeID)
    {
        string pokemonURL = basePokeURL + "pokemon/" + pokeID.ToString();

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        yield return pokeInfoRequest.SendWebRequest();

        if(pokeInfoRequest.result==UnityWebRequest.Result.ConnectionError 
            || pokeInfoRequest.result==UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        //Get pokemon name
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);
        string pokeName = pokeInfo["name"];

        //Get pokemon sprite URL
        string pokeSpriteURL = pokeInfo["sprites"]["front_default"];

        //Get pokemon types in reverse
        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypeNames = new string[pokeTypes.Count];

        for(int i=0,j=pokeTypes.Count-1;i<pokeTypes.Count;i++,j--)
        {
            pokeTypeNames[i] = pokeTypes[j]["type"]["name"];
        }

        //Get pokemon image
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteURL);
        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.result == UnityWebRequest.Result.ConnectionError
            || pokeSpriteRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }

        //Set UI
        PokeImage.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        PokeName.text = pokeName;
        for(int i=0;i<pokeTypes.Count;i++)
        {
            PokeTypes[i].text = pokeTypeNames[i];
        }
    }
}
