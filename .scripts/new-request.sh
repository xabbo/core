#!/bin/bash

set -e
endl=$'\n'

dirIncoming="src/Xabbo.Core/Messages/Incoming"
dirOutgoing="src/Xabbo.Core/Messages/Outgoing"

clientType=$(gum choose --header="Client type?" Any Modern Origins)
gum style "Client type: ${clientType}"

case $clientType in
  Modern|Origins)
    dirIncoming="${dirIncoming}/${clientType}"
    dirOutgoing="${dirOutgoing}/${clientType}"
    ;;
esac

requestMsg=$(gum input --header="Request message name:")
if [[ $requestMsg != *Msg ]]; then
  requestMsg="${requestMsg}Msg"
fi
gum style "Request message name: ${requestMsg}"

requestMsgFile="${dirOutgoing}/${requestMsg}.cs"
if [[ -f "$requestMsgFile" ]]; then
  gum style --foreground=9 "File already exists: ${requestMsgFile}"
  exit 1
fi

requestIdentifier=$(gum input --header="Request identifier:" --placeholder="${requestMsg%Msg}")
gum style "Request identifier: ${requestIdentifier:=${requestMsg%Msg}}"

responseMsg=$(gum input --header="Response message name:")
if [[ $responseMsg != *Msg ]]; then
  responseMsg="${responseMsg}Msg"
fi
gum style "Response message name: ${responseMsg}"

responseMsgFile="${dirIncoming}/${responseMsg}.cs"
if [[ -f "$responseMsgFile" ]]; then
  gum style --foreground=9 "File already exists: ${responseMsgFile}"
  exit 1
fi

responseIdentifier=$(gum input --header="Response identifier:" --placeholder="${responseMsg%Msg}")
gum style "Response identifier: ${responseIdentifier:=${responseMsg%Msg}}"

responseData=$(gum input --header="Response data type:")
gum style "Response data type: ${responseData}"

if [[ -n $clientType ]]; then
  supportedClientsReq="static ClientType IMessage<$requestMsg>.SupportedClients => ClientType.$clientType;${endl}    "
  supportedClientsRes="static ClientType IMessage<$responseMsg>.SupportedClients => ClientType.$clientType;${endl}    "
fi

# Output request source

cat << EOF > $requestMsgFile
using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Request for <see cref="$responseMsg"/>.
/// </summary>
public sealed record $requestMsg : IRequestMessage<$requestMsg, $responseMsg, $responseData>
{
    ${supportedClientsReq}static Identifier IMessage<$requestMsg>.Identifier => Out.$requestIdentifier;
    $responseData IResponseData<$responseMsg, $responseData>.GetData($responseMsg msg) => msg.$responseData;
    static $requestMsg IParser<$requestMsg>.Parse(in PacketReader p)
    {
        throw new System.NotImplementedException();
    }
    void IComposer.Compose(in PacketWriter p)
    {
        throw new System.NotImplementedException();
    }
}
EOF

# Output response source

cat << EOF > $responseMsgFile
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Response for <see cref="Outgoing.$requestMsg"/>.
/// </summary>
public sealed record $responseMsg($responseData $responseData) : IMessage<$responseMsg>
{
    ${supportedClientsRes}static Identifier IMessage<$responseMsg>.Identifier => In.$responseIdentifier;
    static $responseMsg IParser<$responseMsg>.Parse(in PacketReader p) => new(p.Parse<$responseData>());
    void IComposer.Compose(in PacketWriter p) => p.Compose($responseData);
}
EOF
