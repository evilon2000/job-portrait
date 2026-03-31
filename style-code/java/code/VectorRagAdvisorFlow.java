package com.alibaba.cloud.ai.portfolio.springaialibaba;

import org.springframework.ai.chat.client.ChatClient;
import org.springframework.ai.chat.client.advisor.vectorstore.QuestionAnswerAdvisor;
import org.springframework.ai.chat.memory.ChatMemory;
import org.springframework.ai.vectorstore.SearchRequest;
import reactor.core.publisher.Flux;

/**
 * Snippet from SAARAGService4VectorStore:
 * advisor-based vector RAG.
 */
public class VectorRagAdvisorFlow {

    private final ChatClient client;

    public VectorRagAdvisorFlow(ChatClient client) {
        this.client = client;
    }

    public Flux<String> ragChat(String chatId, String prompt, Object vectorStore) {
        return client.prompt()
            .user(prompt)
            .advisors(memoryAdvisor -> memoryAdvisor.param(ChatMemory.CONVERSATION_ID, chatId))
            .advisors(QuestionAnswerAdvisor.builder(vectorStore)
                .searchRequest(SearchRequest.builder().topK(6).build())
                .build())
            .stream().content();
    }
}
