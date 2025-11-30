package io.github.jnetbloon.amproject.model.services;

public class ImageBlock {
    private String imageContentType;
    private byte[] imageData;

    public ImageBlock(String imageContentType, byte[] imageData) {
        this.imageContentType = imageContentType;
        this.imageData = imageData;
    }

    public String getImageContentType() {
        return imageContentType;
    }

    public void setImageContentType(String imageContentType) {
        this.imageContentType = imageContentType;
    }

    public byte[] getImageData() {
        return imageData;
    }

    public void setImageData(byte[] imageData) {
        this.imageData = imageData;
    }
}
