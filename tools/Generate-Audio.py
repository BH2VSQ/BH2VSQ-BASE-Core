from pathlib import Path
from math import pi, sin
import struct
import wave

root = Path(__file__).resolve().parents[1] / "Assets" / "BH2VSQ_BASE" / "Audio"
root.mkdir(parents=True, exist_ok=True)
rate = 22050

for name, notes in {
    "Broadcast_Normal": [(660, 0.12)],
    "Broadcast_Important": [(660, 0.15), (880, 0.15)],
    "Broadcast_Emergency": [(880, 0.20), (660, 0.20), (880, 0.20)],
}.items():
    samples = []
    for frequency, duration in notes:
        count = int(rate * duration)
        for i in range(count):
            envelope = min(1.0, i / 300, (count - i) / 600)
            sample = int(9000 * envelope * sin(2 * pi * frequency * i / rate))
            samples.append(sample)
        samples.extend([0] * int(rate * 0.06))
    with wave.open(str(root / f"{name}.wav"), "wb") as output:
        output.setnchannels(1)
        output.setsampwidth(2)
        output.setframerate(rate)
        output.writeframes(struct.pack("<" + "h" * len(samples), *samples))
