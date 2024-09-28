using System;

using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when updating the user's profile or account.
/// <para/>
/// Supported clients: <see cref="ClientType.Origins"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.UPDATE"/></item>
/// </list>
/// </summary>
public sealed record UpdateProfileMsg : IMessage<UpdateProfileMsg>
{
    public enum FieldId : short
    {
        ParentAgree = 1,
        Name,
        Password,
        Figure,
        Gender,
        Motto,
        Email,
        Birthday,
        DirectMail,
        HasReadAgreement,
        IspId,
        PartnerSite,
        OldPassword,
        OnlineStatus,
        PublicProfileEnabled,
        FriendRequestsEnabled,
        OfflineMessagingEnabled,
        TotpCode
    }

    static ClientType IMessage<UpdateProfileMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<UpdateProfileMsg>.Identifier => Out.UPDATE;

    public bool? ParentAgree { get; set; }
    public string? Name { get; set; }
    public string? Password { get; set; }
    public string? Figure { get; set; }
    public string? Gender { get; set; }
    public string? Motto { get; set; }
    public string? Email { get; set; }
    public string? Birthday { get; set; }
    public bool? DirectMail { get; set; }
    public bool? HasReadAgreement { get; set; }
    public string? IspId { get; set; }
    public string? PartnerSite { get; set; }
    public string? OldPassword { get; set; }
    public bool? OnlineStatus { get; set; }
    public bool? PublicProfileEnabled { get; set; }
    public bool? FriendRequestsEnabled { get; set; }
    public bool? OfflineMessagingEnabled { get; set; }
    public string? TotpCode { get; set; }

    static UpdateProfileMsg IParser<UpdateProfileMsg>.Parse(in PacketReader p)
    {
        UpdateProfileMsg msg = new();
        while (p.Available > 0)
        {
            FieldId fieldId = (FieldId)p.ReadShort();
            switch (fieldId)
            {
                case FieldId.ParentAgree: msg.ParentAgree = p.ReadBool(); break;
                case FieldId.Name: msg.Name = p.ReadString(); break;
                case FieldId.Password: msg.Password = p.ReadString(); break;
                case FieldId.Figure: msg.Figure = p.ReadString(); break;
                case FieldId.Gender: msg.Gender = p.ReadString(); break;
                case FieldId.Motto: msg.Motto = p.ReadString(); break;
                case FieldId.Email: msg.Email = p.ReadString(); break;
                case FieldId.Birthday: msg.Birthday = p.ReadString(); break;
                case FieldId.DirectMail: msg.DirectMail = p.ReadBool(); break;
                case FieldId.HasReadAgreement: msg.HasReadAgreement = p.ReadBool(); break;
                case FieldId.IspId: msg.IspId = p.ReadString(); break;
                case FieldId.PartnerSite: msg.PartnerSite = p.ReadString(); break;
                case FieldId.OldPassword: msg.OldPassword = p.ReadString(); break;
                case FieldId.OnlineStatus: msg.OnlineStatus = p.ReadBool(); break;
                case FieldId.PublicProfileEnabled: msg.PublicProfileEnabled = p.ReadBool(); break;
                case FieldId.FriendRequestsEnabled: msg.FriendRequestsEnabled = p.ReadBool(); break;
                case FieldId.OfflineMessagingEnabled: msg.OfflineMessagingEnabled = p.ReadBool(); break;
                case FieldId.TotpCode: msg.TotpCode = p.ReadString(); break;
                default: throw new Exception($"Unknown field ID when parsing UpdateMsg: {fieldId}.");
            }
        }
        return msg;
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (ParentAgree is { } parentAgree)
        {
            p.WriteShort((short)FieldId.ParentAgree);
            p.WriteBool(parentAgree);
        }

        if (Name is not null)
        {
            p.WriteShort((short)FieldId.Name);
            p.WriteString(Name);
        }

        if (Password is not null)
        {
            p.WriteShort((short)FieldId.Password);
            p.WriteString(Password);
        }

        if (Figure is not null)
        {
            p.WriteShort((short)FieldId.Figure);
            p.WriteString(Figure);
        }

        if (Gender is not null)
        {
            p.WriteShort((short)FieldId.Gender);
            p.WriteString(Gender);
        }

        if (Motto is not null)
        {
            p.WriteShort((short)FieldId.Motto);
            p.WriteString(Motto);
        }

        if (Email is not null)
        {
            p.WriteShort((short)FieldId.Email);
            p.WriteString(Email);
        }

        if (Birthday is not null)
        {
            p.WriteShort((short)FieldId.Birthday);
            p.WriteString(Birthday);
        }

        if (DirectMail is { } directMail)
        {
            p.WriteShort((short)FieldId.DirectMail);
            p.WriteBool(directMail);
        }

        if (HasReadAgreement is { } hasReadAgreement)
        {
            p.WriteShort((short)FieldId.HasReadAgreement);
            p.WriteBool(hasReadAgreement);
        }

        if (IspId is not null)
        {
            p.WriteShort((short)FieldId.IspId);
            p.WriteString(IspId);
        }

        if (PartnerSite is not null)
        {
            p.WriteShort((short)FieldId.PartnerSite);
            p.WriteString(PartnerSite);
        }

        if (OldPassword is not null)
        {
            p.WriteShort((short)FieldId.OldPassword);
            p.WriteString(OldPassword);
        }

        if (OnlineStatus is { } onlineStatus)
        {
            p.WriteShort((short)FieldId.OnlineStatus);
            p.WriteBool(onlineStatus);
        }

        if (PublicProfileEnabled is { } publicProfileEnabled)
        {
            p.WriteShort((short)FieldId.PublicProfileEnabled);
            p.WriteBool(publicProfileEnabled);
        }

        if (FriendRequestsEnabled is { } friendRequestsEnabled)
        {
            p.WriteShort((short)FieldId.FriendRequestsEnabled);
            p.WriteBool(friendRequestsEnabled);
        }

        if (OfflineMessagingEnabled is { } offlineMessagingEnabled)
        {
            p.WriteShort((short)FieldId.OfflineMessagingEnabled);
            p.WriteBool(offlineMessagingEnabled);
        }

        if (TotpCode is not null)
        {
            p.WriteShort((short)FieldId.TotpCode);
            p.WriteString(TotpCode);
        }
    }
}