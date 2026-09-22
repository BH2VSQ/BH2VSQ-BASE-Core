# Security boundary

This is a VRChat Udon world system. Its TOTP secrets are serialized into an uploaded world and can be extracted by a client. Udon behaviours, synchronized player rank/area state, and owner transfers are client controlled. A modified client can bypass local checks or forge a broadcast or floor change. TOTP here is a convenience and social access mechanism, not secure authentication.

Do not store real credentials, personal data, paid entitlements, or sensitive admin capabilities in this world. Do not reuse TOTP secrets from other services. Keep example/default secrets blank. Put per-world secrets only on a scene instance that is kept out of public source control; uploading the world still exposes them to clients.

PlayerData is persistent and useful for experience/preferences, but it is not a trusted leaderboard source. The authoring project does not provide an external server or verified identities. Multi-client tests must include malicious or conflicting ownership assumptions if the world will be public.

This repository is public-source friendly: no TOTP secret or VRChat login token should be committed. Before any public push, inspect `git diff --cached` and Unity scene/prefab changes for accidentally serialized secrets.
