using UnityEngine;
using Mirror;

public class Tagger : NetworkBehaviour
{
    void Start()
    {
        if (isServer)
        {
            CmdSetTag();
        }
    }

    [Command]
    void CmdSetTag()
    {
        gameObject.tag = "Pickup";
        RpcSetTag("Pickup");
    }

    [ClientRpc]
    void RpcSetTag(string tagName)
    {
        gameObject.tag = tagName;
    }
}
