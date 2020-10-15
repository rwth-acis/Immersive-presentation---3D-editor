using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class PhotonConnectionScript : MonoBehaviourPunCallbacks
{
    public PresentHandling presentHandling;
    /// <summary>
    /// Seconds that tht room will stay open no one is in the room anymore
    /// </summary>
    public int roomTTL = 0;

    private string _roomName;
    private string roomName
    {
        get
        {
            return _roomName;
        }

        set
        {
            print(value);
            _roomName = value;
        }
    }
    private bool setupDone = false;

    //The names of the room porperties used in the photon rooms
    const string ANCHORID_PROPERTY_NAME = "azureAnchorId";
    const string STAGE_INDEX_PROPERTY_NAME = "stageIndex";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.EmptyRoomTtl = roomTTL * 1000;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public void connectToRoom(string pRoomName)
    {
        roomName = pRoomName;
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToRegion("eu");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("Created Room");
        //roomEntered();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("Joined Room");
        roomEntered();
    }

    private async void roomEntered()
    {
        if (presentHandling.isOwner)
        {
            print("owner sets up the room");
            //check for spatial anchor existens
            object azureSpatialAnchorIdHelper = null;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(ANCHORID_PROPERTY_NAME, out azureSpatialAnchorIdHelper))
            {
                //Load spatial anchor
                await presentHandling.loadAnchorAsync((string)azureSpatialAnchorIdHelper);
            }
            else
            {
                //Create and upload spatial anchor
                string anchorIdstring = await presentHandling.generateAnchorAsync();
                PhotonNetwork.CurrentRoom.SetCustomProperties(
                    new ExitGames.Client.Photon.Hashtable()
                    {
                        {ANCHORID_PROPERTY_NAME, anchorIdstring }
                    }
                    );
            }

            //check for actualStage
            object stageIndexHelper = null;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(STAGE_INDEX_PROPERTY_NAME, out stageIndexHelper))
            {
                //load actualStage
                int stageIndex = (int)stageIndexHelper;
                presentHandling.stageIndex = stageIndex;
                await presentHandling.loadSceneFromStage(stageIndex);
            }
            else
            {
                //set actual stage
                int stageIndex = 0;
                presentHandling.stageIndex = stageIndex;
                await presentHandling.loadSceneFromStage(stageIndex);
                PhotonNetwork.CurrentRoom.SetCustomProperties(
                    new ExitGames.Client.Photon.Hashtable()
                    {
                        {STAGE_INDEX_PROPERTY_NAME, stageIndex }
                    }
                    );
            }
            //remove loading indicator
            presentHandling.loadingIndicator.SetActive(false);

            //Else
            //Set actualStage to 0
            //load actualStage
            setupDone = true;
        }
        else
        {
            setupDone = true;
            //not the owner
            print("not the owner");
        }
    }

    public  async override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        print("Room properties changed");
        if (setupDone)
        {
            if (propertiesThatChanged.ContainsKey(ANCHORID_PROPERTY_NAME))
            {
                print("Anchor prop changed");
                object anchorIdHelper = null;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(STAGE_INDEX_PROPERTY_NAME, out anchorIdHelper))
                {
                    await presentHandling.loadAnchorAsync((string)anchorIdHelper);
                    presentHandling.loadingIndicator.SetActive(false);
                }
            }

            if (propertiesThatChanged.ContainsKey(STAGE_INDEX_PROPERTY_NAME))
            {
                print("StageIndex changed");
                object stageIndexHelper = null;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(STAGE_INDEX_PROPERTY_NAME, out stageIndexHelper))
                {
                    presentHandling.stageIndex = (int) stageIndexHelper;
                    await presentHandling.loadSceneFromStage((int) stageIndexHelper);
                    presentHandling.loadingIndicator.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Sets the room property of the stage index to the given stage index.
    /// </summary>
    /// <param name="pStageIndex">The stage index that should be set as the room property</param>
    public void sendStageIndex(int pStageIndex)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(
                    new ExitGames.Client.Photon.Hashtable()
                    {
                        {STAGE_INDEX_PROPERTY_NAME, pStageIndex }
                    }
                    );
    }

    public void sendAnchorId(string pAnchorId)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(
                    new ExitGames.Client.Photon.Hashtable()
                    {
                        {STAGE_INDEX_PROPERTY_NAME, pAnchorId }
                    }
                    );
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        print("Player entered the room");
    }

}
