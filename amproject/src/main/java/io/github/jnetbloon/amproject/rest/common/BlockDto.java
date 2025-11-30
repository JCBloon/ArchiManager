package io.github.jnetbloon.amproject.rest.common;

import java.util.List;

public class BlockDto<T> {
    private List<T> items;
    private boolean hasNext;

    public BlockDto(List<T> items, boolean hasNext) {
        this.items = items;
        this.hasNext = hasNext;
    }

    public List<T> getItems() {
        return items;
    }

    public void setItems(List<T> items) {
        this.items = items;
    }

    public boolean isHasNext() {
        return hasNext;
    }

    public void setHasNext(boolean hasNext) {
        this.hasNext = hasNext;
    }
}
