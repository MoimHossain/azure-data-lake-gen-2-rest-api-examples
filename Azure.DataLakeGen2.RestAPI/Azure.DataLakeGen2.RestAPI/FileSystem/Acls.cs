

using System;
using System.Collections.Generic;
using System.Text;

namespace Azure.DataLakeGen2.RestAPI.FileSystem
{

    // Sets POSIX access control rights on files and directories.
    // The value is a comma-separated list of access control entries 
    // that fully replaces the existing access control list(ACL). 
    // Each access control entry(ACE) consists of a scope, a type, 
    // a user or group identifier, and permissions 
    // in the format "[scope:][type]:[id]:[permissions]". 
    // The scope must be "default" to indicate the ACE belongs to 
    // the default ACL for a directory; otherwise scope is implicit 
    // and the ACE belongs to the access ACL.
    // There are four ACE types: "user" grants rights to the 
    // owner or a named user, "group" grants rights to the owning group 
    // or a named group, "mask" restricts rights granted to named users and 
    // the members of groups, and "other" grants rights 
    // to all users not found in any of the other entries.
    // The user or group identifier is omitted for entries 
    // of type "mask" and "other". 
    // The user or group identifier is also omitted for the owner and owning group.
    // The permission field is a 3-character sequence where 
    // the first character is 'r' to grant read access, 
    // the second character is 'w' to grant write access, 
    // and the third character is 'x' to grant execute permission. 
    // If access is not granted, the '-' character is used to denote that the permission is denied.
    // For example, the following ACL grants read, write, and execute rights 
    // to the file owner and john.doe @contoso, the read right to 
    // the owning group, and nothing to everyone else: 
    // "user::rwx,user:john.doe@contoso:rwx,group::r--,other::---,mask=rwx". 
    public enum AclType
    {
        User,
        Group,
        Other,
        Mask
    }
    public enum AclScope
    {
        Access,
        Default
    }

    [FlagsAttribute]
    public enum GrantType : short
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4
    };

    public class AclEntry
    {
        public AclEntry(AclScope scope, AclType type, string upnOrObjectId, GrantType grant)
        {
            Scope = scope;
            AclType = type;
            UpnOrObjectId = upnOrObjectId;
            Grant = grant;
        }

        public AclScope Scope { get; private set; }
        public AclType AclType { get; private set; }
        public string UpnOrObjectId { get; private set; }
        public GrantType Grant { get; private set; }

        public string GetGrantPosixFormat()
        {
            return $"{(this.Grant.HasFlag(GrantType.Read) ? 'r' : '-')}{(this.Grant.HasFlag(GrantType.Write) ? 'w' : '-')}{(this.Grant.HasFlag(GrantType.Execute) ? 'x' : '-')}";
        }

        public override string ToString()
        {
            return $"{(this.Scope == AclScope.Default ? "default:" : string.Empty)}{this.AclType.ToString().ToLowerInvariant()}:{this.UpnOrObjectId}:{GetGrantPosixFormat()}";
        }
    }
}
