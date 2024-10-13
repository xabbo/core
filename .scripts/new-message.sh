#!/bin/bash

set -e
endl=$'\n'

direction=$(gum choose --header="Direction?" Incoming Outgoing)
gum style "Direction: ${direction}"

case $direction in
  Incoming)
    shortDir=In
    ;;
  Outgoing)
    shortDir=Out
    ;;
esac

clientType=$(gum choose --header="Client type?" All Modern Origins)
gum style "Client type: ${clientType}"

filePath="src/Xabbo.Core/Messages/${direction}"

msgName=$(gum input --header="Message name:")
if [[ $msgName != *Msg ]]; then
  msgName="${msgName}Msg"
fi
gum style "Message name: ${msgName}"

filePath="${filePath}/${msgName}.cs"
if [[ -f "$filePath" ]]; then
  gum style --foreground=9 "File already exists: ${filePath}"
  exit 1
fi

if [[ -n $clientType ]]; then
  supportedClients="static ClientType IMessage<$msgName>.SupportedClients => ClientType.$clientType;${endl}    "
fi

identifier=$(gum input --header="Identifier:" --placeholder="${msgName%Msg}")
gum style "Identifier: ${identifier:=${msgName%Msg}}"

# Output source

cat << EOF > $filePath
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.${direction};

public sealed record $msgName : IMessage<$msgName>
{
    ${supportedClients}static Identifier IMessage<$msgName>.Identifier => $shortDir.$identifier;
    static $msgName IParser<$msgName>.Parse(in PacketReader p)
    {
        throw new System.NotImplementedException();
    }
    void IComposer.Compose(in PacketWriter p)
    {
        throw new System.NotImplementedException();
    }
}
EOF
