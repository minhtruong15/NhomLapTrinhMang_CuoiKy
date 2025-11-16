namespace Shared
{
    public enum Command
    {
        // ==== AUTH ====
        AuthRegister,
        AuthLogin,
        AuthOk,
        AuthFail,

        // ==== FRIEND LIST / CHAT ====
        RequestFriendList,      // client -> server
        UpdateFriendList,       // server -> client

        // ==== PRIVATE CHAT ====
        PrivateMessage,          // gửi tin riêng
        RequestPrivateHistory,   // client -> server
        PrivateHistory,          // server -> client

        // ==== FRIEND SEARCH & REQUEST ====
        SearchUser,              // client -> server
        SearchUserResult,        // server -> client
        SendFriendRequest,       // client -> server
        FriendRequestSent,       // server -> client xác nhận đã gửi
        FriendRequestReceived,   // server -> client (người được mời)
        AcceptFriendRequest,     // client -> server
        FriendListChanged,       // server -> client refresh list

        // ==== ADMIN ====
        GetAllUsers,             // server: admin lấy danh sách user
        SavePrivateMessage,      // server lưu tin nhắn riêng
        GetPrivateMessages,      // server trả lịch sử chat

    }
}
