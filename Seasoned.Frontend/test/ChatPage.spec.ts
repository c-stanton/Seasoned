import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import ChatPage from "@/pages/chat.vue" 

const vuetify = createVuetify({ components, directives })

global.ResizeObserver = class ResizeObserver {
  observe() {} unobserve() {} disconnect() {}
};

global.Element.prototype.scrollTo = vi.fn();

vi.stubGlobal('useRuntimeConfig', () => ({
  public: { apiBase: 'http://localhost:5000/' }
}))

const mockRouter = { push: vi.fn(), resolve: vi.fn(() => ({ href: '' })) }
vi.stubGlobal('useRouter', () => mockRouter)

const mockFetch = vi.fn()
vi.stubGlobal('$fetch', mockFetch)

describe('ChatPage.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    global.Element.prototype.scrollTo = vi.fn()
  })

  const mountOptions = {
    global: { 
      plugins: [vuetify],
      stubs: { RecipeDisplay: true },
      provide: { 'router': mockRouter }
    }
  }

  it('shows the placeholder when chat is empty', () => {
    const wrapper = mount(ChatPage, mountOptions)
    expect(wrapper.text()).toContain('"What shall we create today?"')
  })

  it('adds a user message and clears input on send', async () => {
    const wrapper = mount(ChatPage, mountOptions)
    const vm = wrapper.vm as any
    vm.userQuery = 'How do I make a roux?'
    mockFetch.mockResolvedValueOnce({ reply: 'Test', recipe: null })
    
    await vm.askChef()

    expect(vm.chatMessages[0].text).toBe('How do I make a roux?')
    expect(vm.userQuery).toBe('')
  })

  it('displays the assistant reply from the .NET API', async () => {
    mockFetch.mockResolvedValueOnce({
      reply: 'A roux is equal parts flour and fat.',
      recipe: null
    })
    const wrapper = mount(ChatPage, mountOptions)
    const vm = wrapper.vm as any
    vm.userQuery = 'Tell me about roux'
    await vm.askChef()
    expect(vm.chatMessages[1].text).toContain('equal parts flour')
  })

  it('shows error message if the API fails', async () => {
    mockFetch.mockRejectedValueOnce(new Error('Backend Down'))
    const wrapper = mount(ChatPage, mountOptions)
    const vm = wrapper.vm as any
    vm.userQuery = 'Help!'
    await vm.askChef()
    expect(wrapper.text()).toContain('The kitchen is currently closed')
  })
})