from PIL import Image
import os

jpg_folder = "D:/cap_dataset_aug"
png_folder = "D:/cap_dataset_aug_png"
os.makedirs(png_folder, exist_ok=True)

for fname in os.listdir(jpg_folder):
    if fname.lower().endswith(".jpg"):
        img = Image.open(os.path.join(jpg_folder, fname))
        img.save(os.path.join(png_folder, os.path.splitext(fname)[0] + ".png"))