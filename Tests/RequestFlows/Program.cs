using BH2VSQ.Base;
using TMPro;
using VRC.SDKBase;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}

static TeleportRequestManager NewManager()
{
    var manager = new TeleportRequestManager
    {
        teleport = new TeleportManager(),
        panel = new RequestPanel
        {
            noticeText = new TMP_Text(),
            countText = new TMP_Text(),
            rowObjects = new[] { new UnityEngine.GameObject(), new UnityEngine.GameObject() },
            requesterTexts = new[] { new TMP_Text(), new TMP_Text() },
            locationTexts = new[] { new TMP_Text(), new TMP_Text() },
            acceptActions = new[] { new UIButtonAction(), new UIButtonAction() },
            rejectActions = new[] { new UIButtonAction(), new UIButtonAction() }
        }
    };
    manager.panel.requests = manager;
    return manager;
}

static void CopyState(TeleportRequestManager from, TeleportRequestManager to)
{
    Array.Copy(from.requestIds, to.requestIds, TeleportRequestManager.Capacity);
    Array.Copy(from.requesterIds, to.requesterIds, TeleportRequestManager.Capacity);
    Array.Copy(from.targetIds, to.targetIds, TeleportRequestManager.Capacity);
    Array.Copy(from.requestTypes, to.requestTypes, TeleportRequestManager.Capacity);
    Array.Copy(from.states, to.states, TeleportRequestManager.Capacity);
    Array.Copy(from.expiryTicks, to.expiryTicks, TeleportRequestManager.Capacity);
}

var sender = new VRCPlayerApi { playerId = 1, displayName = "发起者" };
var receiver = new VRCPlayerApi { playerId = 2, displayName = "接收者" };
var third = new VRCPlayerApi { playerId = 3, displayName = "第三人" };
VRCPlayerApi.TestPlayers = new[] { sender, receiver, third };
var outgoing = NewManager();
var incoming = NewManager();

Networking.LocalPlayer = sender;
Check(outgoing.Send(receiver.playerId, 0), "first request was not sent");
int firstId = outgoing.requestIds[0];
Check(!outgoing.Send(receiver.playerId, 0), "duplicate pending request was accepted");
CopyState(outgoing, incoming);
Networking.LocalPlayer = receiver;
incoming.OnDeserialization();
Check(incoming.panel.noticeText.text.Contains("按住 Tab"), "receiver did not get Tab notice");
Check(incoming.panel.rowObjects[0].activeSelf && incoming.panel.requesterTexts[0].text == "发起者" && incoming.panel.acceptActions[0].value == firstId, "request list row did not map to incoming request");
incoming.Accept(firstId);
Check(incoming.states[0] == 2, "request was not accepted");
CopyState(incoming, outgoing);
Networking.LocalPlayer = sender;
outgoing.OnDeserialization();
Check(sender.teleportCount == 1, "accepted go-to request did not teleport sender");
Check(outgoing.panel.noticeText.text.Contains("已同意"), "sender did not receive accepted result");

Check(outgoing.Send(receiver.playerId, 1), "invite was not sent");
int secondId = outgoing.requestIds[0];
CopyState(outgoing, incoming);
Networking.LocalPlayer = receiver;
incoming.OnDeserialization();
incoming.Reject(secondId);
CopyState(incoming, outgoing);
Networking.LocalPlayer = sender;
outgoing.OnDeserialization();
Check(sender.teleportCount == 1, "rejected invite teleported sender");
Check(outgoing.panel.noticeText.text.Contains("已拒绝"), "sender did not receive rejected result");

Check(outgoing.Send(receiver.playerId, 0), "new request could not reuse completed slot");
Check(outgoing.Send(third.playerId, 0), "second simultaneous request was not stored");
Check(outgoing.requestIds[0] != outgoing.requestIds[1], "simultaneous requests share an ID");
Console.WriteLine("Request accept/reject results, teleport, duplicate prevention, and simultaneous slots passed.");
