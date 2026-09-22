"""Install TextMesh Pro Essential Resources from Unity's local package cache."""
from pathlib import Path
import sys
import tarfile


def main() -> None:
    root = Path(__file__).resolve().parent.parent
    archives = sorted((root / "Library" / "PackageCache").glob(
        "com.unity.textmeshpro@*/Package Resources/TMP Essential Resources.unitypackage"
    ))
    if not archives:
        raise SystemExit("TextMesh Pro Essential Resources archive is unavailable in Library/PackageCache")
    with tarfile.open(archives[-1]) as archive:
        members = {member.name: member for member in archive.getmembers()}
        for name, member in members.items():
            if not name.endswith("/pathname"):
                continue
            pathname = archive.extractfile(member).read().decode("utf-8").strip()
            relative = Path(pathname)
            if relative.parts[:2] != ("Assets", "TextMesh Pro") or ".." in relative.parts:
                raise SystemExit(f"Unexpected archive path: {pathname}")
            target = root / relative
            target.parent.mkdir(parents=True, exist_ok=True)
            prefix = name[: -len("pathname")]
            content = members.get(prefix + "asset")
            if content is None:
                target.mkdir(exist_ok=True)
            elif not target.exists():
                target.write_bytes(archive.extractfile(content).read())
            meta = members.get(prefix + "asset.meta")
            if meta is not None and not Path(str(target) + ".meta").exists():
                Path(str(target) + ".meta").write_bytes(archive.extractfile(meta).read())
    print("TextMesh Pro Essential Resources installed from local package cache")


if __name__ == "__main__":
    main()
