# spring-ai-alibaba-playground-vector-rag-advisor

## Source

- File: `service/SAARAGService4VectorStore.java`
- Focus: vector-store advisor injection into chat flow

## Snippet

```java
return client.prompt()
    .user(prompt)
    .advisors(memoryAdvisor -> memoryAdvisor.param(ChatMemory.CONVERSATION_ID, chatId))
    .advisors(
        QuestionAnswerAdvisor.builder(vectorStoreDelegate.getVectorStore(vectorStoreType))
            .searchRequest(SearchRequest.builder().topK(6).build())
            .build()
    ).stream().content();
```

## Why Keep It

- Clean RAG composition with advisor-based extension
