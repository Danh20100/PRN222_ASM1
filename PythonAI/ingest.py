import sys
import os
import json
import fitz  # PyMuPDF
from sentence_transformers import SentenceTransformer

# Tắt cảnh báo symlinks của huggingface để console sạch sẽ
os.environ["HF_HUB_DISABLE_SYMLINKS_WARNING"] = "1"
os.environ["TOKENIZERS_PARALLELISM"] = "false"

def extract_text_pdf(file_path):
    text = ""
    try:
        doc = fitz.open(file_path)
        for page in doc:
            text += page.get_text() + "\n"
    except Exception as e:
        # Nếu lỗi in rỗng
        pass
    return text

def chunk_text(text, words_per_chunk=300):
    words = text.split()
    chunks = []
    for i in range(0, len(words), words_per_chunk):
        chunk = " ".join(words[i:i + words_per_chunk])
        chunks.append(chunk)
    return chunks

def main():
    if len(sys.argv) < 2:
        print(json.dumps({"error": "No file path provided"}))
        return

    file_path = sys.argv[1]
    
    if not os.path.exists(file_path):
        print(json.dumps({"error": "File not found"}))
        return

    text = ""
    ext = os.path.splitext(file_path)[1].lower()
    if ext == ".pdf":
        text = extract_text_pdf(file_path)
    elif ext == ".txt":
        with open(file_path, "r", encoding="utf-8") as f:
            text = f.read()
    else:
        print(json.dumps({"error": f"Unsupported file extension {ext}"}))
        return

    if not text.strip():
        print(json.dumps({"error": "No text extracted"}))
        return

    chunks = chunk_text(text, 300)

    # Load model nhẹ để nhúng
    try:
        model = SentenceTransformer("all-MiniLM-L6-v2")
    except Exception as e:
        print(json.dumps({"error": f"Failed to load model: {str(e)}"}))
        return

    results = []
    
    # Băm từng chunk
    for chunk in chunks:
        # Encode trả về mảng numpy, cần đổi sang list float
        vector = model.encode(chunk).tolist()
        results.append({
            "content": chunk,
            "vector": vector
        })

    # In ra console dạng JSON
    print(json.dumps(results))

if __name__ == "__main__":
    main()
