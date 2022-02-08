using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

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

    public Button RandomCard;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    private void OnEnable()
    {
        RandomCard.onClick.AddListener(() =>
        {
            GetRandomPokemon();
        });
    }

    void GetRandomPokemon()
    {
        int randompokeID = Random.Range(1, 808);
        PokeImage.texture = Texture2D.blackTexture;
        PokeID.text = "#" + randompokeID;
        PokeName.text = "Loading...";

        foreach(TMP_Text type in PokeTypes)
        {
            type.text = "";
        }

        StartCoroutine(GetPokemonAtIndex(randompokeID));
    }

    IEnumerator GetPokemonAtIndex(int randompokeID)
    {
        string pokemonURL = basePokeURL + "pokemon/" + randompokeID.ToString();

        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        yield return pokeInfoRequest.SendWebRequest();

        if(pokeInfoRequest.result==UnityWebRequest.Result.ConnectionError 
            || pokeInfoRequest.result==UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }

        PokeCard card = new PokeCard();
        card = JsonUtility.FromJson<PokeCard>(pokeInfoRequest.downloadHandler.text);
        PokeName.text = card.pokemonName;
        for(int i=0;i<PokeTypes.Length;i++)
        {
            PokeTypes[i].text = card.pokemonTypes[i];
        }
    }
}
